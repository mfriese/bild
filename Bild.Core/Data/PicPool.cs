using Bild.Core.Environment;
using MetadataExtractor;

namespace Bild.Core.Data
{
	public class PicPool
	{
		public PicPool(Settings settings)
			=> Settings = settings;

		private Settings Settings { get; }

		public void Import(string directory)
		{
			var files = System.IO.Directory.EnumerateFiles(directory);

			foreach (var file in files)
			{
				if (file == "." || file == "..")
					continue;

				try
				{
					var pic = new Pic(file);

					CopyPicture(pic);
				}
				catch (ImageProcessingException)
				{
					// Maybe faster to identify type first?
				}
			}
		}

		protected void CopyPicture(Pic pic)
		{
			InitializeMediaDirectory(pic.DateTime);

			var targetFolder = Settings.GetFolder(MediaType.Picture, pic.DateTime);
			var targetFilename = pic.DateTime.ToString("yyyyMMdd-hhmmss") + $"{pic.Extension}";
			var targetPath = Path.Combine(targetFolder, targetFilename);

			File.Copy(pic.Path, targetPath);
		}

		protected void InitializeMediaDirectory(DateTime dateTime)
		{
			if (!System.IO.Directory.Exists(Settings.ProjectFolder))
				throw new Exception("Project folder not existing.");

			if (!System.IO.Directory.Exists(Settings.GetMediaFolder()))
				System.IO.Directory.CreateDirectory(Settings.GetMediaFolder());

			if (!System.IO.Directory.Exists(Settings.GetGroupFolder(dateTime)))
				System.IO.Directory.CreateDirectory(Settings.GetGroupFolder(dateTime));

			var mediaTypes = new MediaType[]
			{
				MediaType.Picture,
				MediaType.Video,
				MediaType.Audio
			};

			foreach (var mediaType in mediaTypes)
				if (!System.IO.Directory.Exists(Settings.GetFolder(mediaType, dateTime)))
					System.IO.Directory.CreateDirectory(Settings.GetFolder(mediaType, dateTime));
		}
	}
}
