namespace Bild.Core.Importer
{
	public static class Finder
	{
		public static List<Item> FindAll(string directory)
		{
			var findings = from ff in Directory.EnumerateFiles(directory) select new Item(ff);

			foreach (var dir in Directory.EnumerateDirectories(directory))
			{
				if (dir == "." || dir == "..")
					continue;

				findings = findings.Concat(FindAll(dir));
			}

			return new List<Item>(from ff in findings where ff.IsMedia select ff);
		}
	}
}
