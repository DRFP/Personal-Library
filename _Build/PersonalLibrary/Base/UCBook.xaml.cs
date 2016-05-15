using Library.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Base {
    public partial class UCBook : UserControl {
        public event Action<UCBook> onClick, onClickRemove;
        public Book book;

        public UCBook(Book book) {
            InitializeComponent();
            Margin = new Thickness(5, 5, 5, 5);

            this.book = book;
            imgThumbnail.Source = new BitmapImage(new Uri(book.booThumbnail));
        }

        private void grdBase_MouseUp(object sender, MouseButtonEventArgs e) { onClick?.Invoke(this); }

        private void imgRemove_MouseUp(object sender, MouseButtonEventArgs e) { onClickRemove?.Invoke(this); }
    }
}