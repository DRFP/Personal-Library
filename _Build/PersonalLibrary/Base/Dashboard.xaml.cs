using System;
using System.Windows;
using System.Windows.Input;
using static Library.API;

namespace Base {
    public partial class Dashboard : Window {
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


        private void tbxNewShelf_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (!editShelf) AddShelf(tbxNewShelf.Text);
                else EditShelf(shelfID, tbxNewShelf.Text);

                grdNewShelf.Visibility = Visibility.Collapsed;
                editShelf = false;
            }
        }


        private void imgMinimize_MouseUp(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        private void imgClose_MouseUp(object sender, MouseButtonEventArgs e) { Environment.Exit(0); }
    }
}