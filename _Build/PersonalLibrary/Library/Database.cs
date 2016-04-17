using Library.Model;
using System.Collections.Generic;

namespace Library {
    public class Database {
        public static List<Shelf> Shelves() {
            var shelves = new List<Shelf>();

            shelves.Add(new Shelf { slfName = "Reading now" });
            shelves.Add(new Shelf { slfName = "To read" });
            shelves.Add(new Shelf { slfName = "Have read" });

            return shelves;
        }
    }
}