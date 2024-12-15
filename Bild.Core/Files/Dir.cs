using Bild.Core.Environment;
using System.Collections.ObjectModel;

namespace Bild.Core.Files
{
	public class Dir
	{
		public Dir(string path)
		{
			AbsolutePath = Path.GetFullPath(path);
		}

		public string AbsolutePath { get; }

		public IEnumerable<Dir> Dirs => FindDirectories();
		public IEnumerable<File> Files => FindFiles();

		private IEnumerable<Dir> FindDirectories()
		{
			IEnumerable<Dir> findings;

			if (!Directory.Exists(AbsolutePath))
			{
				findings = [];
			}
			else
			{
				try
				{
					findings = from dd in Directory.EnumerateDirectories(AbsolutePath)
							   where dd != "." && dd != ".."
							   select new Dir(dd);
				}
				catch (UnauthorizedAccessException)
				{
					findings = [];
				}
			}

			return findings;
		}

		private IEnumerable<File> FindFiles()
		{
			IEnumerable<File> findings;

			if (!Directory.Exists(AbsolutePath))
			{
				// No findings, folder does not exist
				findings = [];
			}
			else
			{
				try
				{
					findings = from ff in Directory.EnumerateFiles(AbsolutePath)
							   select new File(ff);
				}
				catch (UnauthorizedAccessException)
				{
					findings = [];
				}
			}

			return findings;
		}
	}
}
