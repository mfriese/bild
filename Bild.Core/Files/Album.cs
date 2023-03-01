using Bild.Core.Environment;
using Bild.Core.Importer;

namespace Bild.Core.Files
{
	public class Album : Dir
	{
		public Album(Settings settings) : base(settings.GetMediaFolder())
		{
			Settings = settings;
		}

		public Settings Settings { get; }

		public void ImportItem(ImportItem item)
		{
			// Get the directory for the year from the media dir
			Dir yearDir = GetOrCreateDir($"{item.Meta.Year}");

			// Determine where to put the new item and get that folder
			var mediaName = MediaTypeHelper.GetMediaName(item.Meta.MediaType);

			// Get the directory for the item, this is where we'll put it
			Dir targetDir = yearDir.GetOrCreateDir(mediaName);

			// create a filename for the given item ...
			var filename = Settings.GetFileName(item.FileType, item.Meta.DateTime);
			
			// Compare filenames for now, if there a conflicts
			if (targetDir.Files.Any(ff => Equals(ff.Filename, filename)))
			{
				// If supposed to skip or undetermined: do not overwrite!
				if (item.Treatment == ImportTreatment.Skip ||
					item.Treatment == ImportTreatment.Unknown)
					return;
			}

			// Build the full path of where to copy the file
			var targetPath = Path.Combine(targetDir.AbsolutePath, filename);

			// Do copy
			System.IO.File.Copy(item.AbsolutePath, targetPath);

			// Add to file datastructure
			targetDir.Files.Add(new File(targetPath));
		}
	}
}
