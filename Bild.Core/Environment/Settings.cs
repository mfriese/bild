using MetadataExtractor.Util;

namespace Bild.Core.Environment
{
	public class Settings
	{
		public string ProjectFolder { get; set; } = string.Empty;

		public string FileNamePattern { get; set; } = "yyyyMMdd-hhmmss";
	}
}
