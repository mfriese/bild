using Bild.Core.Environment;

namespace Bild.Core.Files
{
	public class Dir
	{
		public Dir(string path)
		{
			MediaPath = path;
			MediaName = Path.GetFileName(path) ??
				throw new Exception("Directory name cannot be determined!");
		}

		protected IEnumerable<Dir>? m_dirs = null;
		public IEnumerable<Dir> Dirs
		{
			get => m_dirs ?? FindDirectories(ref m_dirs, MediaPath);
			set => m_dirs = value;
		}

		protected IEnumerable<File>? m_files = null;
		public IEnumerable<File> Files
		{
			get => m_files ?? FindFiles(ref m_files, this);
			set => m_files = value;
		}

		public MediaType MediaType => MediaTypeHelper.GetMediaType(MediaName);
		public string MediaName { get; }
		public string MediaPath { get; }

		private static IEnumerable<Dir> FindDirectories(ref IEnumerable<Dir>? dirs, string path)
		{
			dirs = from dd in Directory.EnumerateDirectories(path)
				   where dd != "." && dd != ".."
				   select new Dir(dd);
			return dirs;
		}

		private static IEnumerable<File> FindFiles(ref IEnumerable<File>? files, Dir dir)
		{
			// Find files in this directory
			files = from ff in Directory.EnumerateFiles(dir.MediaPath) select new File(ff);
			// Find files in all subdirectories, if any
			files = files.Concat(from subDir in dir.Dirs from subFile in subDir.Files select subFile);
			// keep and return
			return files;
		}

		public override string ToString()
			=> $"{MediaType} {MediaPath}";
	}
}
