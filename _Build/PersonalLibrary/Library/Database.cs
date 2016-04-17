using Library.Model;
using System;
using System.Collections.Generic;

namespace Library {
    public class Database {
        public static List<Book> Books() {
            var books = new List<Book>();

            books.Add(new Book {
                slfID = 1,
                booTitle = "Title",
                booDescription = "Description",
                booAuthor = "Author",
                booPublisher = "Publisher",
                booPublishedDate = DateTime.Now.ToString(),
                booPageCount = 100,
                booRating = 5.0,
                booRatingsCount = 1000,
                booInformationURL = "Information URL",
                booPreviewURL = "Preview URL"
            });

            return books;
        }

        public static List<Shelf> Shelves() {
            var shelves = new List<Shelf>();

            shelves.Add(new Shelf { slfName = "Reading now" });
            shelves.Add(new Shelf { slfName = "To read" });
            shelves.Add(new Shelf { slfName = "Have read" });

            return shelves;
        }
    }
}