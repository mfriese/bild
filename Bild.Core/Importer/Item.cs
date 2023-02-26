using Bild.Core.Data;
using Bild.Core.Environment;
using MetadataExtractor.Util;

namespace Bild.Core.Importer
{
	public class Item
	{
		public Item(string absolutePath)
		{
			AbsolutePath = absolutePath;
		}

		public string AbsolutePath { get; set; }

		public bool IsMedia => FileType != FileType.Unknown;

		private FileType? m_fileType;
		public FileType FileType => InitializeFileType(ref m_fileType, AbsolutePath);

		private Meta? m_meta;
		public Meta Meta => InitializeMeta(ref m_meta, AbsolutePath);

		public Treatment Treatment { get; set; } = Treatment.Normal;

		protected static FileType InitializeFileType(ref FileType? mediaType, string path)
		{
			if (null != mediaType)
				return mediaType.Value;

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

			mediaType = FileTypeDetector.DetectFileType(stream);

			return mediaType.Value;
		}

		protected static Meta InitializeMeta(ref Meta? meta, string path)
		{
			if (null != meta)
				return meta;

			meta = new Meta(path);

			return meta;
		}
	}

	public static class ItemExtensions
	{
		public static bool Exisits(this Item item, Settings settings)
		{
			var pathInAlbum = settings.GetFilePath(item.Meta.MediaType, item.FileType, item.Meta.DateTime);

			return File.Exists(pathInAlbum);
		}
	}
}
