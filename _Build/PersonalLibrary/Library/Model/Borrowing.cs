using SQLite;
using System;

namespace Library.Model {
	[Table("Borrowings")]
	public class Borrowing {
		[PrimaryKey, AutoIncrement]
		public int borID { get; set; }
		public int booID { get; set; }
		public string borName { get; set; }
		public DateTime borDeliverydDate { get; set; }
		public string borObservations { get; set; }
		public bool borBorrowed { get; set; }
	}
}