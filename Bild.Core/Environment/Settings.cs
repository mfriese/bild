namespace Bild.Core.Environment
{
	public class Settings
	{
		public string ProjectFolder { get; set; } = string.Empty;

		public string MediaFolder => "media";
		public string VideoFolder => SettingsExtension.GetFolderName(MediaType.Video);
		public string PictureFolder => SettingsExtension.GetFolderName(MediaType.Picture);
		public string AudioFolder => SettingsExtension.GetFolderName(MediaType.Audio);
	}

	public enum MediaType
	{
		Picture = 0,
		Video = 1,
		Audio = 2
	}

	public static class SettingsExtension
	{
		private static readonly string[] Folders = new string[]
		{
			"picture",
			"video",
			"audio"
		};

		public static string GetFolderName(MediaType mediaType)
			=> Folders[(int)mediaType];

		public static string GetMediaFolder(this Settings settings)
			=> Path.Combine(settings.ProjectFolder, settings.MediaFolder);

		public static string GetGroupFolder(this Settings settings, DateTime dateTime)
			=> Path.Combine(settings.GetMediaFolder(), $"{dateTime.Year}");

		public static string GetFolder(this Settings settings, MediaType type, DateTime dateTime)
			=> Path.Combine(settings.GetGroupFolder(dateTime), GetFolderName(type));
	}
}
