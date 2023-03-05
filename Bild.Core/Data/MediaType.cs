using MetadataExtractor.Util;

namespace Bild.Core.Data
{
	public enum MediaType
	{
		Unknown = 0,
		Picture = 1,
		Video = 2,
		Audio = 3,
		Text = 4
	}

	public static class MediaTypeHelper
	{
		internal static readonly string[] Folders = new string[]
		{
			"unknown",
			"picture",
			"video",
			"audio",
			"text"
		};

		internal static readonly Dictionary<FileTypeExt, MediaType> MediaMap = new()
		{
			{ FileTypeExt.Unknown, MediaType.Unknown },
			{ FileTypeExt.Jpeg, MediaType.Picture },
			{ FileTypeExt.Tiff, MediaType.Picture },
			{ FileTypeExt.Psd, MediaType.Picture },
			{ FileTypeExt.Png, MediaType.Picture },
			{ FileTypeExt.Bmp, MediaType.Picture },
			{ FileTypeExt.Gif, MediaType.Picture },
			{ FileTypeExt.Ico, MediaType.Picture },
			{ FileTypeExt.Pcx, MediaType.Picture },
			{ FileTypeExt.Riff, MediaType.Picture },
			{ FileTypeExt.Wav, MediaType.Audio },
			{ FileTypeExt.Avi, MediaType.Video },
			{ FileTypeExt.WebP, MediaType.Video },
			{ FileTypeExt.Arw, MediaType.Picture },
			{ FileTypeExt.Crw, MediaType.Picture },
			{ FileTypeExt.Cr2, MediaType.Picture },
			{ FileTypeExt.Nef, MediaType.Picture },
			{ FileTypeExt.Orf, MediaType.Picture },
			{ FileTypeExt.Raf, MediaType.Picture },
			{ FileTypeExt.Rw2, MediaType.Picture },
			{ FileTypeExt.QuickTime, MediaType.Video },
			{ FileTypeExt.Netpbm, MediaType.Video },
			{ FileTypeExt.Crx, MediaType.Picture },
			{ FileTypeExt.Eps, MediaType.Picture },
			{ FileTypeExt.Tga, MediaType.Picture },
			{ FileTypeExt.Mp3, MediaType.Audio },
			{ FileTypeExt.Heif, MediaType.Picture },
			{ FileTypeExt.Mp4, MediaType.Video },
			{ FileTypeExt.Text, MediaType.Text }
		};

		private static readonly Dictionary<string, FileTypeExt> Types = new();

		public static MediaType GetMediaType(string mediaTypeName)
			=> (from ii in Enumerable.Range(0, Folders.Length)
				where string.Equals(Folders[ii], mediaTypeName, StringComparison.InvariantCultureIgnoreCase)
				select (MediaType)ii).FirstOrDefault(MediaType.Unknown);

		public static string GetMediaName(MediaType mediaType)
			=> Folders[(int)mediaType];

		public static string GetMediaName(FileTypeExt fileType)
			=> GetMediaName(GetMediaType(fileType));

		public static MediaType GetMediaType(FileTypeExt fileType)
		{
			if (MediaMap.TryGetValue(fileType, out MediaType mediaType))
				return mediaType;

			return MediaType.Unknown;
		}

		public static FileTypeExt FindFileTypeExt(string extension)
		{
			InitializeAllExtensions();

			if (string.IsNullOrEmpty(extension))
				return FileTypeExt.Unknown;

			if (Types.TryGetValue(extension, out FileTypeExt fileType))
				return fileType;

			return FileTypeExt.Unknown;
		}

		private static void InitializeAllExtensions()
		{
			if (0 < Types.Count)
				return;

			foreach (var type in (FileTypeExt[])Enum.GetValues(typeof(FileTypeExt)))
			{
				foreach (var extension in type.GetAllExtensions() ?? Array.Empty<string>())
				{
					if (string.IsNullOrEmpty(extension))
						continue;

					Types.Add(extension, type);
				}
			}
		}

		public static FileTypeExt GetFileType(string absolutePath)
		{
			// Try using library function to identify majority of types
			using var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);

			// This might find our type. It might also fail
			var fileType = FileTypeDetector.DetectFileType(stream);

			if (fileType != FileType.Unknown)
				return (FileTypeExt)fileType;

			// Fall back to identifying via file extension
			var extension = Path.GetExtension(absolutePath);

			// Not extension means file is useless
			if (string.IsNullOrEmpty(extension))
				return FileTypeExt.Unknown;

			// Yes, this might happen
			if (extension[0] == '.')
				extension = extension[1..];

			if (Types.TryGetValue(extension, out FileTypeExt fileTypeExt))
				return fileTypeExt;

			return FileTypeExt.Unknown;
		}
	}
}
