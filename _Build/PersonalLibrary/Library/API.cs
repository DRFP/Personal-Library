using Google.Apis.Books.v1;
using Google.Apis.Services;
using Library.Model;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Library.Configuration;

namespace Library {
    public static class API {
        private static SQLiteAsyncConnection connection = new SQLiteAsyncConnection(databaseName);

        public static async Task<List<Book>> GetSearchedBooks(string searchText) {
            var books = new List<Book>();

            var booksService = new BooksService(new BaseClientService.Initializer {
                ApplicationName = applicationName,
                ApiKey = apiKey,
            });

            var _books = await booksService.Volumes.List(searchText).ExecuteAsync();

            if (_books != null && _books.Items != null) {
                foreach (var book in _books.Items) {
                    books.Add(new Book() {
                        booTitle = book.VolumeInfo.Title,
                        booDescription = book.VolumeInfo.Description,
                        booAuthor = (book.VolumeInfo.Authors != null) ? book.VolumeInfo.Authors.FirstOrDefault() : "",
                        booPublisher = book.VolumeInfo.Publisher,
                        booPublishedDate = book.VolumeInfo.PublishedDate,
                        booPageCount = (book.VolumeInfo.PageCount.HasValue) ? book.VolumeInfo.PageCount.Value : 0,
                        booRating = (book.VolumeInfo.AverageRating.HasValue) ? book.VolumeInfo.AverageRating.Value : 0.0,
                        booRatingsCount = (book.VolumeInfo.RatingsCount.HasValue) ? book.VolumeInfo.RatingsCount.Value : 0,
                        booInformationLink = book.VolumeInfo.InfoLink,
                        booPreviewLink = book.VolumeInfo.PreviewLink,
                        booThumbnail = book.VolumeInfo.ImageLinks.Thumbnail
                    });
                }
            }

            return books;
        }

        public static async Task<List<Shelf>> GetShelves() { return await connection.Table<Shelf>().ToListAsync(); }

        public static async Task<int> AddBook(Book book, int shelfID) {
            await connection.InsertAsync(new Book() {
                slfID = shelfID,
                booTitle = book.booTitle,
                booDescription = book.booDescription,
                booAuthor = book.booAuthor,
                booPublisher = book.booPublisher,
                booPublishedDate = book.booPublishedDate,
                booPageCount = book.booPageCount,
                booRating = book.booRating,
                booRatingsCount = book.booRatingsCount,
                booInformationLink = book.booInformationLink,
                booPreviewLink = book.booPreviewLink,
                booThumbnail = book.booThumbnail
            });

            return 0;
        }

        public static async void AddShelf(string shelfName) { await connection.InsertAsync(new Shelf() { slfName = shelfName }); }
    }
}