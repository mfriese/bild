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

				var picPath = Path.Combine(projectFolder, $"{pic.Year}_{ pic.Month}");

				if (!Directory.Exists(picPath))
					Directory.CreateDirectory(picPath);

				var picDate = pic.DateTime.ToString("yyyyMMdd-hhmmss");
				
				var picDestination = Path.Combine(picPath, $"{picDate}.{pic.Extension}");

				File.Copy(file, picDestination);
			}
		}
	}
}
