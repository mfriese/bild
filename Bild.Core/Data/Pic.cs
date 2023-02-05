using ExifLib;

namespace Bild.Core.Data
{
	public sealed class Pic : IDisposable
	{
		public Pic(string path)
		{
			Path = path;
			ExifReader = new ExifReader(path);
		}

		private string Path { get; }
		private ExifReader ExifReader { get; }

		public string Filename
			=> System.IO.Path.GetFileName(Path);

		private DateTime? m_dateTime;
		public DateTime DateTime
		{
			get
			{
				if (null == m_dateTime)
				{
					ExifReader.GetTagValue(ExifTags.DateTime, out DateTime date);

					m_dateTime = date;
				}

				return m_dateTime ?? DateTime.Now;
			}
		}

		public int Year => DateTime.Year;
		public int Month => DateTime.Month;
		public int Day => DateTime.Day;

		public void Dispose()
			=> ExifReader.Dispose();
	}
}
