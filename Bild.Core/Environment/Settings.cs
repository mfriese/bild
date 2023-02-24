namespace Bild.Core.Environment
{
	public class Settings
	{
		public string ProjectFolder { get; set; } = string.Empty;

		public static string MediaFolder => "media";
		public static string VideoFolder => SettingsExtension.GetFolderName(MediaType.Video);
		public static string PictureFolder => SettingsExtension.GetFolderName(MediaType.Picture);
		public static string AudioFolder => SettingsExtension.GetFolderName(MediaType.Audio);
	}

	public enum MediaType
	{
		Unknown = 0,
		Multiple = 1,
		Picture = 2,
		Video = 3,
		Audio = 4
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
	}

	public static class MediaTypeHelper
	{
		internal static readonly string[] Folders = new string[]
		{
			"unknown",
			"multiple",
			"picture",
			"video",
			"audio"
		};

		public static MediaType GetMediaType(string name)
			=> (from ii in Enumerable.Range(0, Folders.Length)
				where string.Equals(Folders[ii], name, StringComparison.InvariantCultureIgnoreCase)
				select (MediaType)ii).FirstOrDefault(MediaType.Unknown);
	}

}
