using SQLite;

namespace Library.Model {
	[Table("Books")]
	public class Book {
		[PrimaryKey, AutoIncrement]
		public int booID { get; set; }
		public long booISBN { get; set; }
		public int slfID { get; set; }
		public string booTitle { get; set; }
		public string booDescription { get; set; }
		public string booCategory { get; set; }
		public string booAuthor { get; set; }
		public string booPublisher { get; set; }
		public string booPublishedDate { get; set; }
		public int booPageCount { get; set; }
		public int booRating { get; set; }
		public string booInformationLink { get; set; }
		public string booPreviewLink { get; set; }
		public string booThumbnail { get; set; }
		public string booNotes { get; set; }
	}
}