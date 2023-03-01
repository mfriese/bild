namespace Bild.Core.Importer
{
	public static class ImportFinder
	{
		public static List<ImportItem> FindAll(string directory)
		{
			var findings = from ff in Directory.EnumerateFiles(directory) select new ImportItem(ff);

			foreach (var dir in Directory.EnumerateDirectories(directory))
			{
				if (dir == "." || dir == "..")
					continue;

				findings = findings.Concat(FindAll(dir));
			}

			return new List<ImportItem>(from ff in findings where ff.IsMedia select ff);
		}
	}
}
