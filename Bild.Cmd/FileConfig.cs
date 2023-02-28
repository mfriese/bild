using Bild.Core.Environment;

namespace Bild.Cmd
{
	public class FileConfig : IFileConfig
	{
		public string ConfigPath => ".";
		public string ConfigFile => "config.json";
	}
}
