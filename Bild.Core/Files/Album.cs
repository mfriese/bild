using Bild.Core.Environment;
using Bild.Core.Importer;
using MetadataExtractor.Util;

namespace Bild.Core.Files
{
	public class Album : Dir
	{
		public Album(Settings settings) : base(settings.GetMediaFolder())
		{
			Settings = settings;
		}

		public Settings Settings { get; }

		public List<Item> ImportQueue { get; } = new();

		public void PrepareDirImport(string path)
			=> ImportQueue.AddRange(Finder.FindAll(path));

		public void AdjustDirImport(Treatment treatment = Treatment.Overwrite)
			=> ImportQueue.ForEach(ii => ii.Treatment = ii.Exisits(Settings) ? treatment : Treatment.Normal);

		public void ExecuteDirImport()
		{
			ImportQueue.ForEach(ii =>
			{
				InitializeMediaDirectory(ii.Meta.DateTime);

				var destination = Settings.GetFilePath(
					ii.Meta.MediaType, ii.FileType, ii.Meta.DateTime);

				if (ii.Treatment == Treatment.Skip)
					return;

				if (ii.Treatment == Treatment.Overwrite &&
					System.IO.File.Exists(destination))
					System.IO.File.Delete(destination);

				System.IO.File.Copy(ii.AbsolutePath, destination);
			});
		}

		protected void InitializeMediaDirectory(DateTime dateTime)
		{
			if (!Directory.Exists(Settings.ProjectFolder))
				throw new Exception("Project folder not existing.");

			if (!Directory.Exists(Settings.GetMediaFolder()))
				Directory.CreateDirectory(Settings.GetMediaFolder());

			if (!Directory.Exists(Settings.GetGroupFolder(dateTime)))
				Directory.CreateDirectory(Settings.GetGroupFolder(dateTime));

			var mediaTypes = new MediaType[]
			{
				MediaType.Picture,
				MediaType.Video,
				MediaType.Audio
			};

			foreach (var mediaType in mediaTypes)
				if (!Directory.Exists(Settings.GetFolder(mediaType, dateTime)))
					Directory.CreateDirectory(Settings.GetFolder(mediaType, dateTime));
		}
	}
}
