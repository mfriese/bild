namespace Bild.Core.Importer
{
	public static class Finder
	{
		public static IEnumerable<Files.File> FindFiles(Files.Dir root)
		    => root.Dirs.Select(FindFiles).SelectMany(ff => ff).Concat(root.Files);
	}
}
