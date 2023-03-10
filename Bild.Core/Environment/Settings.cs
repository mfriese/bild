using Bild.Core.Data;
using MetadataExtractor.Util;

namespace Bild.Core.Environment
{
	public class Settings
	{
		public string ProjectFolder { get; set; } = string.Empty;

		public string FileNamePattern { get; set; } = "yyyyMMdd-hhmmss";

		public static string MediaFolder => "media";
		public static string VideoFolder => SettingsExtension.GetFolderName(MediaType.Video);
		public static string PictureFolder => SettingsExtension.GetFolderName(MediaType.Picture);
		public static string AudioFolder => SettingsExtension.GetFolderName(MediaType.Audio);
	}

	public static class SettingsExtension
	{
		public static string GetFolderName(MediaType mediaType)
			=> MediaTypeHelper.Folders[(int)mediaType];

		public static string GetMediaFolder(this Settings settings)
			=> Path.Combine(settings.ProjectFolder, Settings.MediaFolder);

		public static string GetGroupFolder(this Settings settings, DateTime dateTime)
			=> Path.Combine(settings.GetMediaFolder(), $"{dateTime.Year}");

		public static string GetFolder(this Settings settings, MediaType type, DateTime dateTime)
			=> Path.Combine(settings.GetGroupFolder(dateTime), GetFolderName(type));

		public static string GetFileName(this Settings settings, FileTypeExt fileType, DateTime dateTime)
		{
			var extension = fileType.GetCommonExtension() ??
				throw new Exception("No known extension for this type!");

			return dateTime.ToString(settings.FileNamePattern) + $".{extension}";
		}

		public static string GetFilePath(this Settings settings, MediaType mediaType, FileTypeExt fileType, DateTime creationDate)
		{
			var fileDir = settings.GetFolder(mediaType, creationDate);
			var fileName = settings.GetFileName(fileType, creationDate);
			return Path.Combine(fileDir, fileName);
		}
	}
}
