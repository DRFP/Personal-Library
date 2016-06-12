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
        private bool editShelf, removeShelf, removeBook;
        private int shelfID;

        public Dashboard() {
            InitializeComponent();
            LoadShelves();
        }

        private async void tbxSearch_KeyDown(object sender, KeyEventArgs e) {
            grdShelf.Visibility = Visibility.Collapsed;
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
            var books = await GetBooks(ucShelf.id);

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

            removeShelf = false;
        }

        private void ucShelf_onClickEdit(UCShelf ucShelf) {
            grdNewShelf.Visibility = Visibility.Visible;
            txbNewShelf.Text = "Edit Shelf";
            tbxNewShelf.Text = "Write here the new shelf name...";
            ((Storyboard)Resources["MoveToUp"]).Begin();
            editShelf = true;
            shelfID = ucShelf.id;
        }

        private void ucShelf_onClickRemove(UCShelf ucShelf) {
            ((Storyboard)Resources["MoveToBottom"]).Begin();
            RemoveShelf(ucShelf.id);
            wplShelves.Children.Remove(ucShelf);
            wplShelfBooks.Children.Clear();
            removeShelf = true;
        }

        private void ucBook_onClick(UCBook ucBook) {
            imgThumbnail.Source = new BitmapImage(new Uri(ucBook.book.booThumbnail));

            txbTitle.Text = ucBook.book.booTitle;
            txbDescription.Text = ucBook.book.booDescription;
            txbAuthor.Text = $"Author: {ucBook.book.booAuthor}";
            txbPublisher.Text = $"Publisher: {ucBook.book.booPublisher}";
            txbPublishedDate.Text = $"Published Date: {ucBook.book.booPublishedDate}";
            txbPageCount.Text = $"Page Count: {ucBook.book.booPageCount}";
            txbRating.Text = $"Rating: {ucBook.book.booRating} ({ucBook.book.booRatingsCount})";

            if (removeBook) ((Storyboard)Resources["MoveToRight"]).Begin();
            else ((Storyboard)Resources["MoveToLeft"]).Begin();

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
            ((Storyboard)Resources["MoveToUp"]).Begin();
        }

        private void tbxNewShelf_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (!editShelf) AddShelf(tbxNewShelf.Text);
                else EditShelf(shelfID, tbxNewShelf.Text);

                grdNewShelf.Visibility = Visibility.Collapsed;
                editShelf = false;
            }
        }

        private void grdBase_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                if (grdNewShelf.Margin == new Thickness(14, 552, 0, 0)) ((Storyboard)Resources["MoveToBottom"]).Begin();
                if (grdBookInformation.Margin == new Thickness(706, 28, 0, 0)) ((Storyboard)Resources["MoveToRight"]).Begin();
            }
            else if (e.Key == Key.Enter) LoadShelves();
        }

        private void imgMinimize_MouseUp(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        private void imgClose_MouseUp(object sender, MouseButtonEventArgs e) { Environment.Exit(0); }
    }
				((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
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
				if (grdBorrowInformation.Margin == new Thickness(281, 200, 0, 0)) ((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
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
				((Storyboard)Resources["MoveBookInformationToRight"]).Begin();
				((Storyboard)Resources["MoveBorrowInformationToDown"]).Begin();
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
}