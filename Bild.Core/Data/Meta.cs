using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Util;

namespace Bild.Core.Data
{
	public sealed class Meta
	{
		public Meta(string path)
			=> AbsolutePath = path;

		public string AbsolutePath { get; }
		public string Filename => Path.GetFileName(AbsolutePath);
		public string Extension
		{
			get
			{
				var extension = Path.GetExtension(AbsolutePath);

				if (string.IsNullOrEmpty(extension))
					return string.Empty;

				if (extension[0] == '.')
					return extension[1..];

				return extension;
			}
		}

		private DateTime? m_dateCreated;
		public DateTime DateCreated
		{
			get
			{
				if (!m_dateCreated.HasValue)
				{
					try
					{
						// This might throw, use file access as a fallback
						var exif = ImageMetadataReader.ReadMetadata(AbsolutePath);

						m_dateCreated = exif.
							OfType<ExifIfd0Directory>().FirstOrDefault()?.
							GetDateTime(ExifDirectoryBase.TagDateTime);
					}
					catch(Exception)
					{
					}
				}

				if (!m_dateCreated.HasValue)
				{
					// If this won't work we will not catch the exception!
					m_dateCreated = File.GetCreationTime(AbsolutePath);
				}

				return m_dateCreated.Value;
			}
		}

		public int Year => DateCreated.Year;
		public int Month => DateCreated.Month;
		public int Day => DateCreated.Day;
	}
}
