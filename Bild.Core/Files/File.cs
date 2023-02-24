namespace Bild.Core.Files
{
	public class File
	{
		public File(string path)
		{
			Path = path;
		}

		public string Path { get; }

		public override string ToString()
			=> $"{Path}";
	}
}
