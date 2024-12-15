using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Util;

namespace Bild.Core.Files
{
	public class File
	{
		public File(string path)
		{
			AbsolutePath = Path.GetFullPath(path);
		}

		public string AbsolutePath { get; }

		public string Filename => Path.GetFileName(AbsolutePath);
		public string Extension => Path.GetExtension(AbsolutePath);
		public FileType? exifFileType;
		public FileType? ExifFileType => exifFileType ??= GetExifFileType();
		private DateTime? exifCreationDate;
		public DateTime? ExifCreationDate => exifCreationDate ??= GetExifCreationDate();
		public DateTime? fileCreationDate;
		public DateTime? FileCreationDate => fileCreationDate ??= GetFileCreationDate();

	    private FileType? GetExifFileType()
		{
			using var stream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);

			return FileTypeDetector.DetectFileType(stream);
		}

		private DateTime? GetExifCreationDate()
		{
			DateTime? creation = null;

			try
			{
				var exif = ImageMetadataReader.ReadMetadata(AbsolutePath);

				creation = exif.
					OfType<ExifIfd0Directory>().FirstOrDefault()?.
					GetDateTime(ExifDirectoryBase.TagDateTime);
			}
			catch (Exception)
			{}

			return creation;
		}

		private DateTime? GetFileCreationDate()
		{
			DateTime? creation = null;

			try
			{
				creation = System.IO.File.GetCreationTime(AbsolutePath);
			}
			catch (Exception)
			{}

			return creation;
		}
	}
}
