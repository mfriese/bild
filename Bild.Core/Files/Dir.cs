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
			get => m_files ?? FindFiles(ref m_files, MediaPath);
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

		private static IEnumerable<File> FindFiles(ref IEnumerable<File>? files, string path)
		{
			files = from ff in Directory.EnumerateFiles(path)
					select new File(ff);
			return files;
		}

		public override string ToString()
			=> $"{MediaType} {MediaPath}";
	}
}
