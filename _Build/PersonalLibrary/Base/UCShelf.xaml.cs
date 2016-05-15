using Library.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Base {
    public partial class UCShelf : UserControl {
        public event Action<UCShelf> onClick, onClickEdit, onClickRemove;
        public Shelf shelf;
        public int id;

        public UCShelf(int id, string name) {
            InitializeComponent();

            Margin = new Thickness(0, 0, 0, 6);

            this.id = id;
            tbxName.Text = name;

            if (tbxName.Text == "Reading now" || tbxName.Text == "To read" || tbxName.Text == "Have read") {
                imgEdit.Visibility = Visibility.Hidden;
                imgRemove.Visibility = Visibility.Hidden;
            }
        }

        private void grdBase_MouseUp(object sender, MouseButtonEventArgs e) { onClick?.Invoke(this); }

        private void imgEdit_MouseUp(object sender, MouseButtonEventArgs e) { onClickEdit?.Invoke(this); }

        private void imgRemove_MouseUp(object sender, MouseButtonEventArgs e) { onClickRemove?.Invoke(this); }
    }
}