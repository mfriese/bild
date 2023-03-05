using MetadataExtractor.Util;

namespace Bild.Core.Data
{
	public enum FileTypeExt
	{
		Unknown = FileType.Unknown,
		Jpeg = FileType.Jpeg,
		Tiff = FileType.Tiff,
		Psd = FileType.Psd,
		Png = FileType.Png,
		Bmp = FileType.Bmp,
		Gif = FileType.Gif,
		Ico = FileType.Ico,
		Pcx = FileType.Pcx,
		Riff = FileType.Riff,
		Wav = FileType.Wav,
		Avi = FileType.Avi,
		WebP = FileType.WebP,
		Arw = FileType.Arw,
		Crw = FileType.Crw,
		Cr2 = FileType.Cr2,
		Nef = FileType.Nef,
		Orf = FileType.Orf,
		Raf = FileType.Raf,
		Rw2 = FileType.Rw2,
		QuickTime = FileType.QuickTime,
		Netpbm = FileType.Netpbm,
		Crx = FileType.Crx,
		Eps = FileType.Eps,
		Tga = FileType.Tga,
		Mp3 = FileType.Mp3,
		Heif = FileType.Heif,
		Mp4 = FileType.Mp4,
        // Here come custom formats -> Gap starting at 100
        Text = 100
	}

    public static class FileTypeExtExtensions
    {
        private static readonly string[] _shortNames =
        {
            "Text"
        };

        private static readonly string[] _longNames =
        {
            "Plain text format"
        };

        private static readonly string?[] _mimeTypes =
        {
            "application/text"
        };

        private static readonly string[]?[] _extensions =
        {
            new[] { "txt", "md" }
        };

        public static string GetName(this FileTypeExt fileType)
        {
            var i = (int)fileType;
            if (i < 100)
                return FileTypeExtensions.GetName((FileType)fileType);

            i -= 100;
            if (i < 0 || i >= _shortNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _shortNames[i];
        }

        public static string GetLongName(this FileTypeExt fileType)
        {
            var i = (int)fileType;
            if (i < 100)
                return FileTypeExtensions.GetLongName((FileType)fileType);

            i -= 100;
            if (i < 0 || i >= _longNames.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _longNames[i];
        }

        public static string? GetMimeType(this FileTypeExt fileType)
        {
            var i = (int)fileType;
            if (i < 100)
                return FileTypeExtensions.GetMimeType((FileType)fileType);

            i -= 100;
            if (i < 0 || i >= _mimeTypes.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _mimeTypes[i];
        }

        public static string? GetCommonExtension(this FileTypeExt fileType)
        {
            var i = (int)fileType;
            if (i < 100)
                return FileTypeExtensions.GetCommonExtension((FileType)fileType);

            i -= 100;
            if (i < 0 || i >= _extensions.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i]?.FirstOrDefault();
        }

        public static IEnumerable<string>? GetAllExtensions(this FileTypeExt fileType)
        {
            var i = (int)fileType;
            if (i < 100)
                return FileTypeExtensions.GetAllExtensions((FileType)fileType);

            i -= 100;
            if (i < 0 || i >= _extensions.Length)
                throw new ArgumentException($"Invalid {nameof(FileType)} enum member.", nameof(fileType));
            return _extensions[i];
        }
    }
}
