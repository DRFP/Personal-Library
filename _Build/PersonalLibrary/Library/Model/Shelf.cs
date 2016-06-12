using SQLite;

namespace Library.Model {
	[Table("Shelves")]
	public class Shelf {
		[PrimaryKey, AutoIncrement]
		public int slfID { get; set; }
		public string slfName { get; set; }
	}
}