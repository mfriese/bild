using Bild.Core.Environment;

namespace Bild.Core.Files
{
	public class Album : Dir
	{
		public Album(Settings settings) : base(settings.GetMediaFolder())
		{
			Settings = settings;
		}

		public Settings Settings { get; }
	}
}
