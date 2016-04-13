using Library.Model;
using SQLite;
using System.Threading.Tasks;
using static Library.Configuration;
using static Library.Database;

namespace Library {
    public class SQLiteManager {
        private static SQLiteAsyncConnection connection;

        static SQLiteManager() { connection = new SQLiteAsyncConnection(DatabaseName); }

        public static async Task CreateDatabase() {
            await connection.CreateTablesAsync<Book, Shelf>();
            await connection.InsertAllAsync(Books());
            await connection.InsertAllAsync(Shelves());
        }
    }
}