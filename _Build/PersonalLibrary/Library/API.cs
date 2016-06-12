using Google.Apis.Books.v1;
using Google.Apis.Services;
using Library.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Library.Configuration;

namespace Library {
	public static class API {
		private static SQLiteAsyncConnection connection = new SQLiteAsyncConnection(databaseName);

		public static async Task<int> AddBook(Book book, int shelfID) {
			await connection.InsertAsync(new Book() {
				booISBN = book.booISBN,
				slfID = shelfID,
				booTitle = book.booTitle,
				booDescription = book.booDescription,
				booCategory = book.booCategory,
				booAuthor = book.booAuthor,
				booPublisher = book.booPublisher,
				booPublishedDate = book.booPublishedDate,
				booPageCount = book.booPageCount,
				booRating = book.booRating,
				booInformationLink = book.booInformationLink,
				booPreviewLink = book.booPreviewLink,
				booThumbnail = book.booThumbnail,
				booNotes = ""
			});

			return 0;
		}

		public static async Task<int> AddBorrowing(int bookID) {
			await connection.InsertAsync(new Borrowing() {
				booID = bookID,
				borName = "",
				borDeliverydDate = DateTime.Now,
				borObservations = "",
				borBorrowed = false
			});

			return 0;
		}

		public static async Task<List<Book>> GetBooks(int shelfID, string bookCategory) {
			if (bookCategory != null && bookCategory != "All")
				return await connection.Table<Book>().Where(b => b.slfID.Equals(shelfID) && b.booCategory.Equals(bookCategory)).ToListAsync();
			else
				return await connection.Table<Book>().Where(b => b.slfID.Equals(shelfID)).ToListAsync();
		}

		public static async Task<Borrowing> GetBorrowing(int bookID) {
			return await connection.Table<Borrowing>().Where(b => b.booID.Equals(bookID)).FirstOrDefaultAsync();
		}

		public static async Task<List<Book>> GetSearchedBooks(string searchText) {
			var books = new List<Book>();
			long bookISBN = 0;

			var booksService = new BooksService(new BaseClientService.Initializer {
				ApplicationName = applicationName,
				ApiKey = apiKey,
			});

			var _books = await booksService.Volumes.List(searchText).ExecuteAsync();

			if (_books != null && _books.Items != null) {
				foreach (var book in _books.Items) {
					foreach (var industryIdentifier in book.VolumeInfo.IndustryIdentifiers)
						if (industryIdentifier.Type == "ISBN_13") bookISBN = long.Parse(industryIdentifier.Identifier);

					books.Add(new Book() {
						booISBN = bookISBN,
						booTitle = book.VolumeInfo.Title,
						booDescription = book.VolumeInfo.Description,
						booCategory = (book.VolumeInfo.Categories != null) ? book.VolumeInfo.Categories.FirstOrDefault() : "Unknown",
						booAuthor = (book.VolumeInfo.Authors != null) ? book.VolumeInfo.Authors.FirstOrDefault() : "",
						booPublisher = book.VolumeInfo.Publisher,
						booPublishedDate = book.VolumeInfo.PublishedDate,
						booPageCount = (book.VolumeInfo.PageCount.HasValue) ? book.VolumeInfo.PageCount.Value : 0,
						booRating = 0,
						booInformationLink = book.VolumeInfo.InfoLink,
						booPreviewLink = book.VolumeInfo.PreviewLink,
						booThumbnail = (book.VolumeInfo.ImageLinks != null) ? book.VolumeInfo.ImageLinks.Thumbnail : null,
						booNotes = ""
					});
				}
			}

			return books;
		}

		public static async Task<bool> CheckIfBookExists(long bookISBN) {
			return (await connection.Table<Book>().Where(b => b.booISBN.Equals(bookISBN)).CountAsync() > 0);
		}

		public static async void RemoveBook(Book book) {
			await connection.DeleteAsync(book);

			var borrowing = await connection.Table<Borrowing>().Where(b => b.booID.Equals(book.booID)).FirstOrDefaultAsync();
			RemoveBorrowing(borrowing);
		}

		private async static void RemoveBorrowing(Borrowing borrowing) {
			await connection.DeleteAsync(borrowing);
		}

		public static async void AddShelf(string shelfName) { await connection.InsertAsync(new Shelf() { slfName = shelfName }); }

		public static async Task<List<Shelf>> GetShelves() { return await connection.Table<Shelf>().ToListAsync(); }

		public static async void RemoveShelf(int shelfID) {
			var shelf = await connection.Table<Shelf>().Where(s => s.slfID.Equals(shelfID)).FirstOrDefaultAsync();
			await connection.DeleteAsync(shelf);

			var books = await connection.Table<Book>().Where(b => b.slfID.Equals(shelfID)).ToListAsync();
			foreach (var book in books)
				RemoveBook(book);
		}
	}
}