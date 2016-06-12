using Library.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static Library.API;
using static Library.Configuration;

namespace Base {
	public partial class Dashboard : Window {
		private static SQLiteAsyncConnection connection = new SQLiteAsyncConnection(databaseName);
		private bool editShelf, removeShelf, removeBook, categoryUnknownExists;
		private List<string> categories = new List<string>();
		private UCShelf currentShelf;
		private UCBook currentBook;
		private int shelfID;

		public Dashboard() {
			InitializeComponent();
			LoadShelves();
			ratRating.onClick += ratRating_onClick;
		}

		private async void ratRating_onClick(UCRating ucRating) {
			var book = await connection.Table<Book>().Where(b => b.booID.Equals(currentBook.book.booID)).FirstOrDefaultAsync();
			book.booRating = ucRating.RatingValue;
			List<Book> books;
			await connection.UpdateAsync(book);

			if (cmbCategories.ItemsSource != null) {
				if (cmbCategories.SelectedValue != null)
					books = await GetBooks(currentShelf.id, cmbCategories.SelectedValue.ToString());
				else
					books = await GetBooks(currentShelf.id, null);

				wplShelfBooks.Children.Clear();
				grdSearchResults.Visibility = Visibility.Collapsed;
				grdShelf.Visibility = Visibility.Visible;

				if (books != null) {
					txbWarning.Visibility = Visibility.Collapsed;
					foreach (var _book in books) {
						var ucBook = new UCBook(_book);
						ucBook.onClick += ucBook_onClick;
						ucBook.onClickRemove += ucBook_onClickRemove;
						wplShelfBooks.Children.Add(ucBook);
					}
				}
				else txbWarning.Visibility = Visibility.Visible;
			}

			txbRating.Text = $"Your Rating: {book.booRating} (1-5)";
		}

		private async void tbxSearch_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				grdShelf.Visibility = Visibility.Collapsed;
				cmbCategories.Visibility = Visibility.Collapsed;
				grdSearchResults.Visibility = Visibility.Visible;

				if (wplSearchedBooks.Children.Count > 0) wplSearchedBooks.Children.Clear();
				if (e.Key == Key.Enter && tbxSearch.Text != string.Empty) {
					var books = await GetSearchedBooks(tbxSearch.Text);
					foreach (var book in books) {
						var ucBookInformation = new UCBookInformation(book);
						wplSearchedBooks.Children.Add(ucBookInformation);
					}
				}
			}
		}

		private async void LoadShelves() {
			var shelves = await GetShelves();
			wplShelves.Children.Clear();

			foreach (var shelf in shelves) {
				var ucShelf = new UCShelf(shelf.slfID, shelf.slfName);
				ucShelf.onClick += ucShelf_onClick;
				ucShelf.onClickEdit += ucShelf_onClickEdit;
				ucShelf.onClickRemove += ucShelf_onClickRemove;
				wplShelves.Children.Add(ucShelf);
			}
		}

		private async void ucShelf_onClick(UCShelf ucShelf) {
			var books = await GetBooks(ucShelf.id, null);
			cmbCategories.ItemsSource = null;
			categories.Clear();

			if (books.Count > 0)
				categories.Add("All");

			foreach (var book in books) {
				if (book.booCategory != "Unknown")
					categories.Add(book.booCategory);
				else
					categoryUnknownExists = true;
			}

			if (categoryUnknownExists)
				categories.Add("Unknown");

			categories = categories.Distinct().ToList();
			categoryUnknownExists = false;
			cmbCategories.ItemsSource = categories;

			if (!removeShelf) {
				wplShelfBooks.Children.Clear();
				grdSearchResults.Visibility = Visibility.Collapsed;
				grdShelf.Visibility = Visibility.Visible;

				if (books != null) {
					txbWarning.Visibility = Visibility.Collapsed;
					foreach (var book in books) {
						var ucBook = new UCBook(book);
						ucBook.onClick += ucBook_onClick;
						ucBook.onClickRemove += ucBook_onClickRemove;
						wplShelfBooks.Children.Add(ucBook);
					}
				}
				else txbWarning.Visibility = Visibility.Visible;
			}

			currentShelf = ucShelf;
			removeShelf = false;

			if (cmbCategories.Items.Count != 0) {
				cmbCategories.Visibility = Visibility.Visible;
				cmbCategories.SelectedIndex = 0;
			}
			else
				cmbCategories.Visibility = Visibility.Hidden;
		}

		private void ucShelf_onClickEdit(UCShelf ucShelf) {
			grdNewShelf.Visibility = Visibility.Visible;
			txbNewShelf.Text = "Edit Shelf";
			tbxNewShelf.Text = "Write here the new shelf name...";
			((Storyboard)Resources["MoveNewShelfToUp"]).Begin();
			editShelf = true;
			shelfID = ucShelf.id;
		}

		private void ucShelf_onClickRemove(UCShelf ucShelf) {
			((Storyboard)Resources["MoveNewShelfToDown"]).Begin();
			RemoveShelf(ucShelf.id);
			wplShelves.Children.Remove(ucShelf);
			wplShelfBooks.Children.Clear();
			removeShelf = true;
		}

		private async void ucBook_onClick(UCBook ucBook) {
			imgThumbnail.Source = new BitmapImage(new Uri(ucBook.book.booThumbnail));
			txbTitle.Tag = ucBook.book.booID;
			txbTitle.Text = ucBook.book.booTitle;
			txbDescription.Text = ucBook.book.booDescription;
			txbAuthor.Text = $"Author: {ucBook.book.booAuthor}";
			txbPublisher.Text = $"Publisher: {ucBook.book.booPublisher}";
			txbPublishedDate.Text = $"Published Date: {ucBook.book.booPublishedDate}";
			txbPageCount.Text = $"Page Count: {ucBook.book.booPageCount}";
			txbRating.Text = $"Your Rating: {ucBook.book.booRating} (1-5)";
			ratRating.RatingValue = ucBook.book.booRating;
			tbxNotes.Text = ucBook.book.booNotes;
			currentBook = ucBook;

			if (removeBook) {
				((Storyboard)Resources["MoveBookInformationToRight"]).Begin();
				((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
			}
			else
				((Storyboard)Resources["MoveBookInformationToLeft"]).Begin();

			txbBorrowingTitle.Text = currentBook.book.booTitle;
			var borrowing = await GetBorrowing(currentBook.book.booID);

			if (borrowing.borBorrowed) {
				tbxName.IsEnabled = false;
				dtpDeliveryDate.IsEnabled = false;
				tbxObservations.IsEnabled = false;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Visible;
			}
			else {
				tbxName.IsEnabled = true;
				dtpDeliveryDate.IsEnabled = true;
				tbxObservations.IsEnabled = true;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Hidden;
			}

			tbxName.Text = borrowing.borName;
			dtpDeliveryDate.SelectedDate = borrowing.borDeliverydDate;
			tbxObservations.Text = borrowing.borObservations;

			removeBook = false;
		}

		private void ucBook_onClickRemove(UCBook ucBook) {
			RemoveBook(ucBook.book);
			wplShelfBooks.Children.Remove(ucBook);
			removeBook = true;
		}

		private void imgAddShelf_MouseUp(object sender, MouseButtonEventArgs e) {
			grdNewShelf.Visibility = Visibility.Visible;
			txbNewShelf.Text = "New Shelf";
			tbxNewShelf.Text = "Write here the new shelf name...";
			((Storyboard)Resources["MoveNewShelfToUp"]).Begin();
		}

		private async void tbxNewShelf_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				if (!editShelf)
					AddShelf(tbxNewShelf.Text);
				else {
					var shelf = await connection.Table<Shelf>().Where(s => s.slfID.Equals(shelfID)).FirstOrDefaultAsync();
					shelf.slfName = tbxNewShelf.Text;
					await connection.UpdateAsync(shelf);
				}

				LoadShelves();
				grdNewShelf.Visibility = Visibility.Collapsed;
				editShelf = false;
			}
		}

		private void grdBase_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				if (grdNewShelf.Margin == new Thickness(14, 552, 0, 0)) ((Storyboard)Resources["MoveNewShelfToDown"]).Begin();
				if (grdBookInformation.Margin == new Thickness(706, 25, 0, 0)) ((Storyboard)Resources["MoveBookInformationToRight"]).Begin();
				if (grdBorrowInformation.Margin == new Thickness(281, 200, 0, 0)) ((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
			}
		}

		private void btnEditNotes_Click(object sender, RoutedEventArgs e) {
			btnSaveNotes.Visibility = Visibility.Visible;
			tbxNotes.IsEnabled = true;
		}

		private async void btnSaveNotes_Click(object sender, RoutedEventArgs e) {
			btnSaveNotes.Visibility = Visibility.Hidden;
			tbxNotes.IsEnabled = false;
			int bookID = int.Parse(txbTitle.Tag.ToString());
			var book = await connection.Table<Book>().Where(b => b.booID.Equals(bookID)).FirstOrDefaultAsync();
			List<Book> books;
			book.booNotes = tbxNotes.Text;
			await connection.UpdateAsync(book);
			if (cmbCategories.ItemsSource != null) {
				if (cmbCategories.SelectedValue != null)
					books = await GetBooks(currentShelf.id, cmbCategories.SelectedValue.ToString());
				else
					books = await GetBooks(currentShelf.id, null);

				wplShelfBooks.Children.Clear();
				grdSearchResults.Visibility = Visibility.Collapsed;
				grdShelf.Visibility = Visibility.Visible;

				if (books != null) {
					txbWarning.Visibility = Visibility.Collapsed;
					foreach (var _book in books) {
						var ucBook = new UCBook(_book);
						ucBook.onClick += ucBook_onClick;
						ucBook.onClickRemove += ucBook_onClickRemove;
						wplShelfBooks.Children.Add(ucBook);
					}
				}
				else txbWarning.Visibility = Visibility.Visible;
			}
		}

		private void btnBorrowInformation_Click(object sender, RoutedEventArgs e) {
			((Storyboard)Resources["MoveBorrowInformationToUp"]).Begin();
		}

		private void btnCloseThisPanel_Click(object sender, RoutedEventArgs e) {
			((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
		}

		private void txbInformationLink_MouseUp(object sender, MouseButtonEventArgs e) {
			Process.Start(currentBook.book.booInformationLink);
		}

		private void txbPreviewLink_MouseUp(object sender, MouseButtonEventArgs e) {
			Process.Start(currentBook.book.booPreviewLink);
		}

		private async void cmbCategories_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			if (cmbCategories.ItemsSource != null) {
				var books = await GetBooks(currentShelf.id, cmbCategories.SelectedValue.ToString());

				wplShelfBooks.Children.Clear();
				grdSearchResults.Visibility = Visibility.Collapsed;
				grdShelf.Visibility = Visibility.Visible;

				if (books != null) {
					txbWarning.Visibility = Visibility.Collapsed;
					foreach (var book in books) {
						var ucBook = new UCBook(book);
						ucBook.onClick += ucBook_onClick;
						ucBook.onClickRemove += ucBook_onClickRemove;
						wplShelfBooks.Children.Add(ucBook);
					}
				}
				else
					txbWarning.Visibility = Visibility.Visible;

				((Storyboard)Resources["MoveBookInformationToRight"]).Begin();
				((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
			}
		}

		private async void btnBorrow_Click(object sender, RoutedEventArgs e) {
			var borrowing = await connection.Table<Borrowing>().Where(b => b.booID.Equals(currentBook.book.booID)).FirstOrDefaultAsync();
			borrowing.borName = tbxName.Text;
			borrowing.borDeliverydDate = dtpDeliveryDate.SelectedDate.Value.Date;
			borrowing.borObservations = tbxObservations.Text;
			borrowing.borBorrowed = true;
			await connection.UpdateAsync(borrowing);

			txbBorrowingTitle.Text = currentBook.book.booTitle;
			var _borrowing = await GetBorrowing(currentBook.book.booID);

			if (_borrowing.borBorrowed) {
				tbxName.IsEnabled = false;
				dtpDeliveryDate.IsEnabled = false;
				tbxObservations.IsEnabled = false;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Visible;
			}
			else {
				tbxName.IsEnabled = true;
				dtpDeliveryDate.IsEnabled = true;
				tbxObservations.IsEnabled = true;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Hidden;
			}

			tbxName.Text = _borrowing.borName;
			dtpDeliveryDate.SelectedDate = _borrowing.borDeliverydDate;
			tbxObservations.Text = _borrowing.borObservations;
		}

		private async void btnIAlreadyReceivedTheBook_Click(object sender, RoutedEventArgs e) {
			Borrowing borrowing = await connection.Table<Borrowing>().Where(b => b.booID.Equals(currentBook.book.booID)).FirstOrDefaultAsync();
			borrowing.borName = "";
			borrowing.borDeliverydDate = DateTime.Now.Date;
			borrowing.borObservations = "";
			borrowing.borBorrowed = false;
			await connection.UpdateAsync(borrowing);

			txbBorrowingTitle.Text = currentBook.book.booTitle;
			var _borrowing = await GetBorrowing(currentBook.book.booID);

			if (_borrowing.borBorrowed) {
				tbxName.IsEnabled = false;
				dtpDeliveryDate.IsEnabled = false;
				tbxObservations.IsEnabled = false;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Visible;
			}
			else {
				tbxName.IsEnabled = true;
				dtpDeliveryDate.IsEnabled = true;
				tbxObservations.IsEnabled = true;
				btnIAlreadyReceivedTheBook.Visibility = Visibility.Hidden;
			}

			tbxName.Text = _borrowing.borName;
			dtpDeliveryDate.SelectedDate = _borrowing.borDeliverydDate;
			tbxObservations.Text = _borrowing.borObservations;
		}

		private void imgMinimize_MouseUp(object sender, MouseButtonEventArgs e) {
			WindowState = WindowState.Minimized;
		}

		private void imgClose_MouseUp(object sender, MouseButtonEventArgs e) {
			Environment.Exit(0);
		}
	}
}