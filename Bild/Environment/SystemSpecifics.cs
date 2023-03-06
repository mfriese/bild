using System;
using System.Runtime.InteropServices;

namespace Bild.Environment
{
	public static class SystemSpecifics
	{
		public static string FileExplorer
		{
			get
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					return "explorer";
				}

				if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					return "open";
				}

				throw new InvalidOperationException("Platform not supported.");
			}
		}
	}
}
