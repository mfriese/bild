using Bild.Core.Environment;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.FileType;

namespace Bild.Core.Data
{
	public sealed class Meta
	{
		public Meta(string path)
		{
			Path = path;

			Exif = ImageMetadataReader.ReadMetadata(path);
		}

		public string Path { get; }
		private IReadOnlyList<MetadataExtractor.Directory> Exif { get; }

		public string Filename
			=> System.IO.Path.GetFileName(Path);

		public string Extension
			=> System.IO.Path.GetExtension(Path);

		private DateTime? m_dateTime;
		public DateTime DateTime
		{
			get
			{
				if (!m_dateTime.HasValue)
				{
					m_dateTime = Exif.
						OfType<ExifIfd0Directory>().FirstOrDefault()?.
						GetDateTime(ExifIfd0Directory.TagDateTime) ?? DateTime.MinValue;
				}

				return m_dateTime.Value;
			}
		}

		private string m_fileType = string.Empty;
		public string FileType
		{
			get
			{
				if (string.IsNullOrEmpty(m_fileType))
				{
					m_fileType = Exif.
						OfType<FileTypeDirectory>().FirstOrDefault()?.
						GetDescription(FileTypeDirectory.TagDetectedFileTypeName) ?? "Unknown";
				}

				return m_fileType;
			}
		}

		public MediaType MediaType { get; } = MediaType.Picture;

		public int Year => DateTime.Year;
		public int Month => DateTime.Month;
		public int Day => DateTime.Day;
	}

	
}
