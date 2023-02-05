using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bild.Core.Context
{
	[Table("Bilder")]
	public class Bild
	{
		[Key]
		public int Id { get; set; }

		[Column("Name")]
		public string? Name { get; set; }

		[Column("CreationDate")]
		public string? DateTime { get; set; }
	}
}
