using Library.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Library.API;

namespace Base {
    public partial class UCBookInformation : UserControl {
        private Book book;

        public UCBookInformation(Book book) {
            InitializeComponent();

            Margin = new Thickness(3, 0, 0, 0);
            this.book = book;

            if (book.booThumbnail != null) imgThumbnail.Source = new BitmapImage(new Uri(book.booThumbnail));

            txbTitle.Text = book.booTitle;
            txbDescription.Text = book.booDescription;
            txbAuthor.Text = $"Author: {book.booAuthor}";
            txbPublisher.Text = $"Publisher: {book.booPublisher}";
            txbPublishedDate.Text = $"Published Date: {book.booPublishedDate}";
            txbPageCount.Text = $"Page Count: {book.booPageCount}";
            txbRating.Text = $"Rating: {book.booRating} ({book.booRatingsCount})";

            SetShelves();
        }

        private async void SetShelves() {
            var shelves = await GetShelves();
            foreach (var shelf in shelves) cmbShelves.Items.Add(new ComboBoxItem() { Tag = shelf.slfID, Content = shelf.slfName });
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e) {
            var shelf = cmbShelves.SelectedItem as ComboBoxItem;
            if (shelf != null) await AddBook(book, int.Parse(shelf.Tag.ToString()));
        }
    }
}