using Library.Model;
using SQLite;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Library.API;
using static Library.Configuration;

namespace Base {
	public partial class UCBookInformation : UserControl {
		private static SQLiteAsyncConnection connection = new SQLiteAsyncConnection(databaseName);
		private Book book;

		public UCBookInformation(Book book) {
			InitializeComponent();

			Margin = new Thickness(3, 0, 0, 0);
			this.book = book;

			if (book.booThumbnail != null)
				imgThumbnail.Source = new BitmapImage(new Uri(book.booThumbnail));

			txbTitle.Text = book.booTitle;
			txbDescription.Text = book.booDescription;
			txbAuthor.Text = $"Author: {book.booAuthor}";
			txbPublisher.Text = $"Publisher: {book.booPublisher}";
			txbPublishedDate.Text = $"Published Date: {book.booPublishedDate}";
			txbPageCount.Text = $"Page Count: {book.booPageCount}";
			SetShelves();
		}

		private async void SetShelves() {
			var shelves = await GetShelves();
			foreach (var shelf in shelves)
				cmbShelves.Items.Add(new ComboBoxItem() { Tag = shelf.slfID, Content = shelf.slfName });
		}

		private async void btnAdd_Click(object sender, RoutedEventArgs e) {
			var shelf = cmbShelves.SelectedItem as ComboBoxItem;
			if (shelf != null) {
				if (!await CheckIfBookExists(book.booISBN)) {
					await AddBook(book, int.Parse(shelf.Tag.ToString()));
					var _books = await connection.Table<Book>().ToListAsync();
					await AddBorrowing(_books.LastOrDefault().booID);
				}
			}
		}

		private void txbInformationLink_MouseUp(object sender, MouseButtonEventArgs e) {
			Process.Start(book.booInformationLink);
		}

		private void txbPreviewLink_MouseUp(object sender, MouseButtonEventArgs e) {
			Process.Start(book.booPreviewLink);
		}
	}
}