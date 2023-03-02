using System;
using Bild.Core.Environment;
using MetadataExtractor.Util;

namespace Bild.Core.Data
{
	public static class Media
	{
		private static Dictionary<string, FileType> Types = new();

		public static MediaType FindMediaType(string absolutePath)
		{
			InitializeAllExtensions();

			var extension = Path.GetExtension(absolutePath);

			if (string.IsNullOrEmpty(extension))
				return MediaType.Unknown;

			if (extension[0] == '.')
				extension = extension.Substring(1);

			if (Types.TryGetValue(extension, out var filetype))
				return MediaType.Picture;

			// TODO: Mediatypes here!

			return MediaType.Unknown;
        }

		private static void InitializeAllExtensions()
		{
			if (0 < Types.Count)
				return;

            foreach (var type in (FileType[])Enum.GetValues(typeof(FileType)))
            {
				foreach (var extension in type.GetAllExtensions() ?? Array.Empty<string>())
				{
					if (string.IsNullOrEmpty(extension))
						continue;

					Types.Add(extension, type);
				}
            }
        }
	}
}

