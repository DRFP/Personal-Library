using System;
using System.Windows;
using System.Windows.Input;
using static Library.API;

namespace Base {
    public partial class Dashboard : Window {
        public Dashboard() {
            InitializeComponent();
            LoadShelves();
        }

        private async void tbxSearch_KeyDown(object sender, KeyEventArgs e) {
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

            foreach (var shelf in shelves) {
                var ucShelf = new UCShelf(shelf.slfID, shelf.slfName);
                wplShelves.Children.Add(ucShelf);
            }
        }

        private void imgAddShelf_MouseUp(object sender, MouseButtonEventArgs e) { grdNewShelf.Visibility = Visibility.Visible; }

        private void tbxNewShelf_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                AddShelf(tbxNewShelf.Text);
                grdNewShelf.Visibility = Visibility.Collapsed;
                wplShelves.Children.Clear();
                LoadShelves();
            }
        }


        private void imgMinimize_MouseUp(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        private void imgClose_MouseUp(object sender, MouseButtonEventArgs e) { Environment.Exit(0); }
    }
}