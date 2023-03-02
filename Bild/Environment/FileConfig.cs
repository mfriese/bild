using Bild.Core.Environment;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Bild.Environment
{
	public class FileConfig : IFileConfig
	{
		private string? m_configPath;
		public string ConfigPath => InitConfigPath(ref m_configPath);

		private string? m_configFile;
		public string ConfigFile => InitConfigFile(ref m_configFile);

		private static string InitConfigPath(ref string? path)
		{
			if (!string.IsNullOrEmpty(path))
				return path;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var appDataId = System.Environment.SpecialFolder.ApplicationData;
				var appDataPath = System.Environment.GetFolderPath(appDataId);
				path = Path.Combine(appDataPath, "Bild");
				return path;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return "~";
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				var msg = "Messagebus implementation for Linux is absent!";
				throw new EntryPointNotFoundException(msg);
			}

			path = string.Empty;
			return path;
		}

		private static string InitConfigFile(ref string? file)
		{
			if (!string.IsNullOrEmpty(file))
				return file;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				file = "settings.json";
				return file;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				file = ".settings.json";
				return file;
			}

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				file = ".settings.json";
				return file;
			}

			file = string.Empty;
			return file;
		}
	}
}
