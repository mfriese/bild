using Bild.Core.Data;
using Bild.Core.Environment;
using MetadataExtractor.Util;

namespace Bild.Core.Importer
{
	public class ImportItem
	{
		public ImportItem(string absolutePath)
		{
			AbsolutePath = absolutePath;
		}

		public string AbsolutePath { get; set; }

		private FileTypeExt? m_fileType;
		public FileTypeExt FileType => InitializeFileType(ref m_fileType, AbsolutePath);

		private Meta? m_meta;
		public Meta Meta => InitializeMeta(ref m_meta, AbsolutePath);

		public ImportTreatment Treatment { get; set; } = ImportTreatment.Unknown;

		protected static FileTypeExt InitializeFileType(ref FileTypeExt? fileType, string absolutePath)
		{
			if (null != fileType)
				return fileType.Value;

			fileType = MediaTypeHelper.GetFileType(absolutePath);

			return fileType.Value;
		}

		protected static Meta InitializeMeta(ref Meta? meta, string path)
		{
			if (null != meta)
				return meta;

			meta = new Meta(path);

			return meta;
		}

		public override string ToString() => $"{AbsolutePath} -> {FileType}, {Treatment}";
	}

	//public static class ItemExtensions
	//{
	//	public static bool Exisits(this ImportItem item, Settings settings)
	//	{
	//		var fileType = MediaTypeHelper.GetFileType(item.AbsolutePath);
	//		var mediaType = MediaTypeHelper.GetMediaType(fileType);

	//		var pathInAlbum = settings.GetFilePath(mediaType, fileType, item.Meta.DateCreated);

	//		return File.Exists(pathInAlbum);
	//	}
	//}
}
