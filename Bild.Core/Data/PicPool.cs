using Bild.Core.Environment;

namespace Bild.Core.Data
{
	public class PicPool
	{
		public PicPool(Repository repo) => Repo = repo;

		private Repository Repo { get; }

		public void Import(string directory)
		{
			var files = Directory.EnumerateFiles(directory, "*.jpg", SearchOption.TopDirectoryOnly);

			var projectFolder = Repo.Settings.ProjectFolder;

			foreach (var file in files)
			{
				using var pic = new Pic(file);

				var yearPath = Path.Combine(projectFolder, $"{pic.Year}");
				var monthPath = Path.Combine(yearPath, $"{pic.Month}");
				var dayPath = Path.Combine(monthPath, $"{pic.Day}");

				if (!Directory.Exists(yearPath))
					Directory.CreateDirectory(yearPath);

				if (!Directory.Exists(monthPath))
					Directory.CreateDirectory(monthPath);

				if (!Directory.Exists(dayPath))
					Directory.CreateDirectory(dayPath);

				File.Copy(file, Path.Combine(dayPath, pic.Filename));
			}
		}
	}
}
