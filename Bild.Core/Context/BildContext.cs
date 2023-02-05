using Microsoft.EntityFrameworkCore;

namespace Bild.Core.Context
{
	public class BildContext : DbContext
	{
		public BildContext()
		{
			DbConnectionString = @"Data Source = pool.sqlite";
		}

		public BildContext(string dbPath)
		{
			DbConnectionString = dbPath;
		}

		private static BildContext? m_instance;
		public static BildContext Instance
		{
			get
			{
				if (null == m_instance)
				{
					m_instance = new BildContext();
				}

				return m_instance;
			}
		}

		public string DbConnectionString { get; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			// connect to sqlite database
			options.UseSqlite(DbConnectionString);
		}

		public void Migrate() => Database.Migrate();

		public DbSet<Bild> Bilder { get; set; }
	}
}
