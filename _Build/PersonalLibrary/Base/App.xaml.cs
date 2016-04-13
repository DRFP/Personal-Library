using System.IO;
using System.Windows;
using static Library.Configuration;
using static Library.SQLiteManager;

namespace Base {
    public partial class App : Application {
        public App() { Configure(); }

        private async void Configure() {
            if (!File.Exists(DatabaseName)) await CreateDatabase();
        }
    }
}