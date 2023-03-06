using Bild.Core.Data;
using Bild.Core.Environment;
using System.Collections.ObjectModel;

namespace Bild.Core.Files
{
	public class Dir
	{
		public Dir(string path)
		{
			AbsolutePath = path;
			Filename = Path.GetFileName(path) ??
				throw new Exception("Directory name cannot be determined!");
		}

		protected ObservableCollection<Dir>? m_dirs;
		public ObservableCollection<Dir> Dirs
		{
			get => m_dirs ?? FindDirectories(ref m_dirs, AbsolutePath);
			set => m_dirs = value;
		}

		protected ObservableCollection<File>? m_files;
		public ObservableCollection<File> Files
		{
			get => m_files ?? FindFiles(ref m_files, this);
			set => m_files = value;
		}

		public MediaType MediaType => MediaTypeHelper.GetMediaType(Filename);
		public string Filename { get; }
		public string AbsolutePath { get; }

		public Dir GetOrCreateDir(string name)
		{
			var dir = Dirs.Where(dd => Equals(dd.Filename, name)).FirstOrDefault();

			if (dir is null)
			{
				var newPath = Path.Combine(AbsolutePath, name);

				Directory.CreateDirectory(newPath);

				dir = new Dir(newPath);

				Dirs.Add(dir);
			}

			return dir;
		}

		private static ObservableCollection<Dir> FindDirectories(ref ObservableCollection<Dir>? dirs, string path)
		{
			IEnumerable<Dir> findings;

			if (!Directory.Exists(path))
			{
				// No findings, folder does not exist
				findings = Array.Empty<Dir>();
			}
			else
			{
				findings = from dd in Directory.EnumerateDirectories(path)
						   where dd != "." && dd != ".."
						   select new Dir(dd);
			}

			// Make this observable ...
			dirs = new ObservableCollection<Dir>(findings);

			return dirs;
		}

		private static ObservableCollection<File> FindFiles(ref ObservableCollection<File>? files, Dir dir)
		{
			var allFiles = Directory.EnumerateFiles(dir.AbsolutePath);

			foreach (var ff in allFiles)
				Console.WriteLine(ff);

			// Find files in this directory
			var findings = from ff in Directory.EnumerateFiles(dir.AbsolutePath)
						   where MediaTypeHelper.GetFileType(ff) != FileTypeExt.Unknown
						   select new File(ff);

			// Find files in all subdirectories, if any
			findings = findings.Concat(from subDir in dir.Dirs from subFile in subDir.Files select subFile);

			// Make this observable ...
			files = new ObservableCollection<File>(findings);

			// keep and return
			return files;
		}

		public override string ToString() => $"{MediaType} {AbsolutePath}";
	}
}
