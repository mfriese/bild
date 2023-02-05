using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ExifLib
{
	/// <summary>
	/// A class for reading Exif data from a JPEG file. The file will be open for reading for as long as the class exists.
	/// <seealso cref="http://gvsoft.homedns.org/exif/Exif-explanation.html"/>
	/// </summary>
	public class ExifReader : IDisposable
    {
        private static readonly Regex _nullDateTimeMatcher = new(@"^[\s0]{4}[:\s][\s0]{2}[:\s][\s0]{5}[:\s][\s0]{2}[:\s][\s0]{2}$");

        private readonly bool _leaveOpen;
        private Stream _stream;
        private BinaryReader _reader;        

        /// <summary>
        /// The main tag id/absolute file offset catalogue
        /// </summary>
        private Dictionary<ushort, long> _ifd0Catalogue;

        /// <summary>
        /// The thumbnail tag id/absolute file offset catalogue
        /// </summary>
        /// <remarks>JPEG images contain 2 main sections - one for the main image (which contains most of the useful EXIF data), and one for the thumbnail
        /// image (which contains little more than the thumbnail itself). This catalogue is only used by <see cref="GetJpegThumbnailBytes"/>.</remarks>
        private Dictionary<ushort, long> _ifd1Catalogue;

        /// <summary>
        /// Indicates whether to read data using big or little endian byte aligns
        /// </summary>
        private bool _isLittleEndian;

        /// <summary>
        /// The position in the filestream at which the TIFF header starts
        /// </summary>
        private long _tiffHeaderStart;

        /// <summary>
        /// The location of the thumbnail IFD
        /// </summary>
        private uint _ifd1Offset;

        private bool _isInitialized;

        public ExifReader(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) { }

        public ExifReader(Stream stream)
            : this(stream, false) { }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ExifReader(Stream stream, bool leaveOpen)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new ExifLibException("ExifLib requires a seekable stream");

            // Leave the stream open if the user wants it so
            _leaveOpen = leaveOpen;
            _stream = stream;
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            // JPEG encoding uses big endian (i.e. Motorola) byte aligns. The TIFF encoding
            // found later in the document will specify the byte aligns used for the
            // rest of the document.
            _isLittleEndian = false;

            // Open the file in a stream            
            _reader = new BinaryReader(_stream, Encoding.UTF8, _leaveOpen);

            // Make sure the file's a JPEG.
            if (ReadUShort() != 0xFFD8)
                throw new ExifLibException("File is not a valid JPEG");

            // Scan to the start of the Exif content
            ReadToExifStart();

            // Create an index of all Exif tags found within the document
            CreateTagIndex();            
        }

        #region TIFF methods

        /// <summary>
        /// Returns the length (in bytes) per component of the specified TIFF data type
        /// </summary>
        /// <returns></returns>
        private static byte GetTIFFFieldLength(ushort tiffDataType)
        {
			return tiffDataType switch
			{
				1 or 2 or 7 or 6 => 1,
				3 or 8 => 2,
				4 or 9 or 11 => 4,
				5 or 10 or 12 => 8,
				_ => throw new ExifLibException(string.Format("Unknown TIFF datatype: {0}", tiffDataType)),
			};
		}

        #endregion

        #region Methods for reading data directly from the filestream

        /// <summary>
        /// Gets a 2 byte unsigned integer from the file
        /// </summary>
        /// <returns></returns>
        private ushort ReadUShort()
        {
            return ToUShort(ReadBytes(2));
        }

        /// <summary>
        /// Gets a 4 byte unsigned integer from the file
        /// </summary>
        /// <returns></returns>
        private uint ReadUint()
        {
            return ToUint(ReadBytes(4));
        }

        private string ReadString(int chars)
        {
            var bytes = ReadBytes(chars);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        private byte[] ReadBytes(int byteCount)
        {
            return _reader.ReadBytes(byteCount);
        }

        /// <summary>
        /// Reads some bytes from the specified TIFF offset
        /// </summary>
        /// <param name="tiffOffset"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        private byte[] ReadBytes(ushort tiffOffset, int byteCount)
        {
            // Keep the current file offset
            var originalOffset = _stream.Position;

            // Move to the TIFF offset and retrieve the data
            _stream.Seek(tiffOffset + _tiffHeaderStart, SeekOrigin.Begin);

            var data = _reader.ReadBytes(byteCount);

            // Restore the file offset
            _stream.Position = originalOffset;

            return data;
        }

        #endregion

        #region Data conversion methods for interpreting datatypes from a byte array

        /// <summary>
        /// Converts 2 bytes to a ushort using the current byte aligns
        /// </summary>
        /// <returns></returns>
        private ushort ToUShort(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        /// Converts 8 bytes to the numerator and denominator
        /// components of an unsigned rational using the current byte aligns
        /// </summary>
        private uint[] ToURationalFraction(byte[] data)
        {
            var numeratorData = new byte[4];
            var denominatorData = new byte[4];

            Array.Copy(data, numeratorData, 4);
            Array.Copy(data, 4, denominatorData, 0, 4);

            var numerator = ToUint(numeratorData);
            var denominator = ToUint(denominatorData);

            return new[] { numerator, denominator };
        }


        /// <summary>
        /// Converts 8 bytes to an unsigned rational using the current byte aligns
        /// </summary>
        /// <seealso cref="ToRational"/>
        private double ToURational(byte[] data)
        {
            var fraction = ToURationalFraction(data);

            return fraction[0] / (double)fraction[1];
        }

        /// <summary>
        /// Converts 8 bytes to the numerator and denominator
        /// components of an unsigned rational using the current byte aligns
        /// </summary>
        /// <remarks>
        /// A TIFF rational contains 2 4-byte integers, the first of which is
        /// the numerator, and the second of which is the denominator.
        /// </remarks>
        private int[] ToRationalFraction(byte[] data)
        {
            var numeratorData = new byte[4];
            var denominatorData = new byte[4];

            Array.Copy(data, numeratorData, 4);
            Array.Copy(data, 4, denominatorData, 0, 4);

            int numerator = ToInt(numeratorData);
            int denominator = ToInt(denominatorData);

            return new[] { numerator, denominator };
        }

        /// <summary>
        /// Converts 8 bytes to a signed rational using the current byte aligns.
        /// </summary>
        /// <seealso cref="ToRationalFraction"/>
        private double ToRational(byte[] data)
        {
            var fraction = ToRationalFraction(data);

            return fraction[0] / (double)fraction[1];
        }

        /// <summary>
        /// Converts 4 bytes to a uint using the current byte aligns
        /// </summary>
        private uint ToUint(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// Converts 4 bytes to an int using the current byte aligns
        /// </summary>
        private int ToInt(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt32(data, 0);
        }

        private double ToDouble(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToDouble(data, 0);
        }

        private float ToSingle(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToSingle(data, 0);
        }

        private short ToShort(byte[] data)
        {
            if (_isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt16(data, 0);
        }

        private sbyte ToSByte(byte[] data)
        {
            // An sbyte should just be a byte with an offset range.
            return (sbyte)(data[0] - byte.MaxValue);
        }

        /// <summary>
        /// Retrieves an array from a byte array using the supplied converter
        /// to read each individual element from the supplied byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="elementLengthBytes"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        private static Array GetArray<T>(byte[] data, int elementLengthBytes, ConverterMethod<T> converter)
        {
            Array convertedData = new T[data.Length / elementLengthBytes];

            var buffer = new byte[elementLengthBytes];

            // Read each element from the array
            for (int elementCount = 0; elementCount < data.Length / elementLengthBytes; elementCount++)
            {
                // Place the data for the current element into the buffer
                Array.Copy(data, elementCount * elementLengthBytes, buffer, 0, elementLengthBytes);

                // Process the data and place it into the output array
                convertedData.SetValue(converter(buffer), elementCount);
            }

            return convertedData;
        }

        /// <summary>
        /// A delegate used to invoke any of the data conversion methods
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>Although this could be defined as covariant, it wouldn't work on Windows Phone</remarks>
        private delegate T ConverterMethod<out T>(byte[] data);

        #endregion

        #region Stream seek methods - used to get to locations within the JPEG

        /// <summary>
        /// Scans to the Exif block
        /// </summary>
        private void ReadToExifStart()
        {
            // The file has a number of blocks (Exif/JFIF), each of which
            // has a tag number followed by a length. We scan the document until the required tag (0xFFE1)
            // is found. All tags start with FF, so a non FF tag indicates an error.

            // Get the next tag.
            byte markerStart;
            byte markerNumber = 0;
            while (((markerStart = _reader.ReadByte()) == 0xFF) && (markerNumber = _reader.ReadByte()) != 0xE1)
            {
                // Get the length of the data.
                ushort dataLength = ReadUShort();

                // Jump to the end of the data (note that the size field includes its own size)!
                _stream.Seek(dataLength - 2, SeekOrigin.Current);
            }

            // It's only success if we found the 0xFFE1 marker
            if (markerStart != 0xFF || markerNumber != 0xE1)
                throw new ExifLibException("Could not find Exif data block");
        }

        /// <summary>
        /// Reads through the Exif data and builds an index of all Exif tags in the document
        /// </summary>
        /// <returns></returns>
        private void CreateTagIndex()
        {
            // The next 4 bytes are the size of the Exif data.
            ReadUShort();

            // Next is the Exif data itself. It starts with the ASCII "Exif" followed by 2 zero bytes.
            if (ReadString(4) != "Exif")
                throw new ExifLibException("Exif data not found");

            // 2 zero bytes
            if (ReadUShort() != 0)
                throw new ExifLibException("Malformed Exif data");

            // We're now into the TIFF format
            _tiffHeaderStart = _stream.Position;

            // What byte align will be used for the TIFF part of the document? II for Intel, MM for Motorola
            _isLittleEndian = ReadString(2) == "II";

            // Next 2 bytes are always the same.
            if (ReadUShort() != 0x002A)
                throw new ExifLibException("Error in TIFF data");

            // Get the offset to the IFD (image file directory)
            var ifdOffset = ReadUint();

            // Note that this offset is from the first byte of the TIFF header. Jump to the IFD.
            _stream.Position = ifdOffset + _tiffHeaderStart;

            // Catalogue this first IFD (there will be another IFD)
            _ifd0Catalogue = new Dictionary<ushort, long>();
            CatalogueIFD(ref _ifd0Catalogue);

            // The address to the IFD1 (the thumbnail IFD) is located immediately after the main IFD
            _ifd1Offset = ReadUint();

			// There's more data stored in the subifd, the offset to which is found in tag 0x8769.
			// As with all TIFF offsets, it will be relative to the first byte of the TIFF header.
			if (!GetTagValue(_ifd0Catalogue, 0x8769, out uint offset))
				throw new ExifLibException("Unable to locate Exif data");

			// Jump to the exif SubIFD
			_stream.Position = offset + _tiffHeaderStart;

            // Add the subIFD to the catalogue too
            CatalogueIFD(ref _ifd0Catalogue);

            // Go to the GPS IFD and catalogue that too. It's an optional
            // section.
            if (GetTagValue(_ifd0Catalogue, 0x8825, out offset))
            {
                // Jump to the GPS SubIFD
                _stream.Position = offset + _tiffHeaderStart;

                // Add the subIFD to the catalogue too
                CatalogueIFD(ref _ifd0Catalogue);
            }

            // Finally, catalogue the thumbnail IFD if it's present
            if (_ifd1Offset != 0)
            {
                _stream.Position = _ifd1Offset + _tiffHeaderStart;
                _ifd1Catalogue = new Dictionary<ushort, long>();
                CatalogueIFD(ref _ifd1Catalogue);
            }
        }
        #endregion

        #region Exif data catalog and retrieval methods

        public bool GetTagValue<T>(ExifTags tag, out T result)
        {            
            return GetTagValue((ushort)tag, out result);
        }

        public bool GetTagValue<T>(ushort tagId, out T result)
        {
            // All useful EXIF tags are stored in the ifd0 catalogue. The ifd1 catalogue is only for thumbnail retrieval.            
            Initialize();
            return GetTagValue(_ifd0Catalogue, tagId, out result);
        }

        /// <summary>
        /// Retrieves an Exif value with the requested tag ID
        /// </summary>
        private bool GetTagValue<T>(IDictionary<ushort, long> tagDictionary, ushort tagId, out T result)
        {
			var tagData = GetTagBytes(tagDictionary, tagId, out ushort tiffDataType, out uint numberOfComponents);

			if (tagData == null)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
				result = default;
#pragma warning restore CS8601 // Possible null reference assignment.
				return false;
            }

            var fieldLength = GetTIFFFieldLength(tiffDataType);

            // Convert the data to the appropriate datatype. Note the weird boxing via object.
            // The compiler doesn't like it otherwise.
            switch (tiffDataType)
            {
                case 1:
                    // unsigned byte
                    if (numberOfComponents == 1)
                        result = (T)(object)tagData[0];
                    else
                        result = (T)(object)tagData;
                    return true;
                case 2:
                    // ascii string
                    var str = Encoding.UTF8.GetString(tagData, 0, tagData.Length);

                    // There may be a null character within the string
                    var nullCharIndex = str.IndexOf('\0');
                    if (nullCharIndex != -1)
                        str = str[..nullCharIndex];

                    // Special processing for dates.
                    if (typeof(T) == typeof(DateTime))
                    {
						var success = ToDateTime(str, out DateTime dateResult);

						result = (T)(object)dateResult;
                        return success;

                    }

                    result = (T)(object)str;
                    return true;
                case 3:
                    // unsigned short
                    if (numberOfComponents == 1)
                        result = (T)(object)ToUShort(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToUShort);
                    return true;
                case 4:
                    // unsigned long
                    if (numberOfComponents == 1)
                        result = (T)(object)ToUint(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToUint);
                    return true;
                case 5:
                    // unsigned rational
                    if (numberOfComponents == 1)
                    {
                        // Special case - sometimes it's useful to retrieve the numerator and
                        // denominator in their raw format
                        if (typeof(T).IsArray)
                            result = (T)(object)ToURationalFraction(tagData);
                        else
                            result = (T)(object)ToURational(tagData);
                    }
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToURational);
                    return true;
                case 6:
                    // signed byte
                    if (numberOfComponents == 1)
                        result = (T)(object)ToSByte(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToSByte);
                    return true;
                case 7:
                    // undefined. Treat it as a byte.
                    if (numberOfComponents == 1)
                        result = (T)(object)tagData[0];
                    else
                        result = (T)(object)tagData;
                    return true;
                case 8:
                    // Signed short
                    if (numberOfComponents == 1)
                        result = (T)(object)ToShort(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToShort);
                    return true;
                case 9:
                    // Signed long
                    if (numberOfComponents == 1)
                        result = (T)(object)ToInt(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToInt);
                    return true;
                case 10:
                    // signed rational
                    if (numberOfComponents == 1)
                    {
                        // Special case - sometimes it's useful to retrieve the numerator and
                        // denominator in their raw format
                        if (typeof(T).IsArray)
                            result = (T)(object)ToRationalFraction(tagData);
                        else
                            result = (T)(object)ToRational(tagData);
                    }
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToRational);
                    return true;
                case 11:
                    // single float
                    if (numberOfComponents == 1)
                        result = (T)(object)ToSingle(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToSingle);
                    return true;
                case 12:
                    // double float
                    if (numberOfComponents == 1)
                        result = (T)(object)ToDouble(tagData);
                    else
                        result = (T)(object)GetArray(tagData, fieldLength, ToDouble);
                    return true;
                default:
                    throw new ExifLibException(string.Format("Unknown TIFF datatype: {0}", tiffDataType));
            }
        }

        private static bool ToDateTime(string str, out DateTime result)
        {
            // From page 28 of the Exif 2.2 spec (http://www.exif.org/Exif2-2.PDF): 

            // "When the field is left blank, it is treated as unknown ... When the date and time are unknown, 
            // all the character spaces except colons (":") may be filled with blank characters"
            if (string.IsNullOrEmpty(str) || _nullDateTimeMatcher.IsMatch(str))
            {
                result = DateTime.MinValue;
                return false;
            }

            // There are 2 types of date - full date/time stamps, and plain dates. Dates are 10 characters long.
            if (str.Length == 10)
            {
                result = DateTime.ParseExact(str, "yyyy:MM:dd", CultureInfo.InvariantCulture);
                return true;
            }

            // "The format is "YYYY:MM:DD HH:MM:SS" with time shown in 24-hour format, and the date and time separated by one blank character [20.H].
            result = DateTime.ParseExact(str, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            return true;
        }

        /// <summary>
        /// Gets the data in the specified tag ID, starting from before the IFD block.
        /// </summary>
        /// <param name="tiffDataType"></param>
        /// <param name="numberOfComponents">The number of items which make up the data item - i.e. for a string, this will be the
        /// number of characters in the string</param>
        /// <param name="tagDictionary"></param>
        /// <param name="tagId"></param>
        private byte[] GetTagBytes(IDictionary<ushort, long> tagDictionary, ushort tagId, out ushort tiffDataType, out uint numberOfComponents)
        {
            // Get the tag's offset from the catalogue and do some basic error checks
            if (_stream == null || _reader == null || tagDictionary == null || !tagDictionary.ContainsKey(tagId))
            {
                tiffDataType = 0;
                numberOfComponents = 0;
#pragma warning disable CS8603 // Possible null reference return.
				return null;
#pragma warning restore CS8603 // Possible null reference return.
			}

            var tagOffset = tagDictionary[tagId];

            // Jump to the TIFF offset
            _stream.Position = tagOffset;

            // Read the tag number from the file
            var currenttagId = ReadUShort();

            if (currenttagId != tagId)
                throw new ExifLibException("Tag number not at expected offset");

            // Read the offset to the Exif IFD
            tiffDataType = ReadUShort();
            numberOfComponents = ReadUint();
            var tagData = ReadBytes(4);

            // If the total space taken up by the field is longer than the
            // 2 bytes afforded by the tagData, tagData will contain an offset
            // to the actual data.
            var dataSize = (int)(numberOfComponents * GetTIFFFieldLength(tiffDataType));

            if (dataSize > 4)
            {
                var offsetAddress = ToUShort(tagData);
                return ReadBytes(offsetAddress, dataSize);
            }

            // The value is stored in the tagData starting from the left
            Array.Resize(ref tagData, dataSize);

            return tagData;
        }

        /// <summary>
        /// Records all Exif tags and their offsets within
        /// the file from the current IFD
        /// </summary>
        private void CatalogueIFD(ref Dictionary<ushort, long> tagOffsets)
        {
            // Assume we're just before the IFD.

            // First 2 bytes is the number of entries in this IFD
            var entryCount = ReadUShort();

            for (ushort currentEntry = 0; currentEntry < entryCount; currentEntry++)
            {
                var currentTagNumber = ReadUShort();

                // Record this in the catalogue
                tagOffsets[currentTagNumber] = _stream.Position - 2;

                // Go to the end of this item (10 bytes, as each entry is 12 bytes long)
                _stream.Seek(10, SeekOrigin.Current);
            }
        }

        #endregion

        #region Thumbnail retrieval
        /// <summary>
        /// Retrieves a JPEG thumbnail from the image if one is present. Note that this method cannot retrieve thumbnails encoded in other formats,
        /// but since the DCF specification specifies that thumbnails must be JPEG, this method will be sufficient for most purposes
        /// See http://gvsoft.homedns.org/exif/exif-explanation.html#TIFFThumbs or http://partners.adobe.com/public/developer/en/tiff/TIFF6.pdf for 
        /// details on the encoding of TIFF thumbnails
        /// </summary>
        /// <returns></returns>
        public byte[] GetJpegThumbnailBytes()
        {
            Initialize();

            if (_ifd1Catalogue == null)
#pragma warning disable CS8603 // Possible null reference return.
				return null;

				// Get the thumbnail encoding
			if (!GetTagValue(_ifd1Catalogue, (ushort)ExifTags.Compression, out ushort compression))
				return null;

			// This method only handles JPEG thumbnails (compression type 6)
			if (compression != 6)
                return null;

			// Get the location of the thumbnail
			if (!GetTagValue(_ifd1Catalogue, (ushort)ExifTags.JPEGInterchangeFormat, out uint offset))
				return null;

			// Get the length of the thumbnail data
			if (!GetTagValue(_ifd1Catalogue, (ushort)ExifTags.JPEGInterchangeFormatLength, out uint length))
				return null;


			_stream.Position = offset;

            // The thumbnail may be padded, so we scan forward until we reach the JPEG header (0xFFD8) or the end of the file
            int currentByte;
            var previousByte = -1;
            while ((currentByte = _stream.ReadByte()) != -1)
            {
                if (previousByte == 0xFF && currentByte == 0xD8)
                    break;

                previousByte = currentByte;
            }

            if (currentByte != 0xD8)
                return null;

            // Step back to the start of the JPEG header
            _stream.Position -= 2;

            var imageBytes = new byte[length];
            _stream.Read(imageBytes, 0, (int)length);

            // A valid JPEG stream ends with 0xFFD9. The stream may be padded at the end with multiple 0xFF bytes.
            var jpegStreamEnd = (int)length - 1;
            while (jpegStreamEnd > 0 && imageBytes[jpegStreamEnd] == 0xFF)
                jpegStreamEnd--;

            if (jpegStreamEnd <= 0 || imageBytes[jpegStreamEnd] != 0xD9 || imageBytes[jpegStreamEnd - 1] != 0xFF)
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            return imageBytes;
        }
        #endregion

        #region IDisposable Members

        ~ExifReader()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_reader != null)
                    _reader.Dispose();

                if (!_leaveOpen)
                {
                    // Make sure the file handle is released                
                    if (_stream != null)
                        _stream.Dispose();
                }
            }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			_reader = null;
			_stream = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      
        }

        #endregion
    }

    public class ExifLibException : Exception
    {
        public ExifLibException()
        {
        }

        public ExifLibException(string message)
            : base(message)
        {
        }

        public ExifLibException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}