using Newtonsoft.Json;

namespace Bild.Core.Environment
{
	/// <summary>
	/// Class to saving and loading application configuration to/from file.
	/// </summary>
	public class Repository
	{
		public Repository(IFileConfig fileConfig) => FileConfig = fileConfig;

		private IFileConfig FileConfig { get; }

		public Settings Settings
		{
			get => LoadFromFile<Settings>(FileConfig.ConfigFilePath);
			set => WriteToFile(value, FileConfig.ConfigFilePath);
		}

		/// <summary>
		/// Load content from file, interpret as json and deserialize.
		/// </summary>
		/// <returns>Configuration as represented in file.</returns>
		private static T LoadFromFile<T>(string filePath) where T : new()
		{
			if (!File.Exists(filePath))
				return new T();

			var configFileContent = File.ReadAllText(filePath);

			try
			{
				// Try deserializing the file content
				var result = JsonConvert.DeserializeObject<T>(configFileContent);

				if (result is null)
					return new T();

				return result;
			}
			catch (Exception)
			{
				// Just return the default values
				return new T();
			}
		}

		/// <summary>
		/// Write content to file and serialize as json before doing so.
		/// </summary>
		/// <param name="config">Configuration to be persisted.</param>
		private static void WriteToFile<T>(T content, string filePath)
		{
			var directoryName = Path.GetDirectoryName(filePath);

			if (!string.IsNullOrEmpty(directoryName) &&
				!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			var jsonContent = JsonConvert.SerializeObject(content, Formatting.Indented);

			File.WriteAllText(filePath, jsonContent);
		}
	}
}
