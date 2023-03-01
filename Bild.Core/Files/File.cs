namespace Bild.Core.Files
{
	public class File
	{
		public File(string absolutePath)
		{
			AbsolutePath = absolutePath;
		}

		public string AbsolutePath { get; }

		public string Filename => Path.GetFileName(AbsolutePath);

		public override string ToString() => $"{AbsolutePath}";
	}
}
