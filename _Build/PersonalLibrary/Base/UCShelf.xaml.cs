using System.Windows;
using System.Windows.Controls;

namespace Base {
    public partial class UCShelf : UserControl {
        public int id;

        public UCShelf(int id, string name) {
            InitializeComponent();

            Margin = new Thickness(0, 0, 0, 6);

            this.id = id;
            tbxName.Text = name;
        }
    }
}