namespace ShoppingHelper.Model
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SQLite.Net;
    using SQLite.Net.Async;

    public static class ConnectionHelper
    {
        #region Static Fields

        private static SQLiteConnection _connection;

        #endregion

        #region Public Methods and Operators

        public static void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        public static SQLiteAsyncConnection GetAsyncConnection()
        {
            SQLiteConnectionWithLock connection =
                new SQLiteConnectionWithLock(
                    new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(),
                    new SQLiteConnectionString(
                        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "shoppinghelper.db3"),
                        storeDateTimeAsTicks: false));

            connection.CreateTable<ShoppingList>();

            connection.CreateTable<Product>();

            string[] preset =
            {
                "Äpfel (Pink Lady)",
                "Äpfel (Braeburn)",
                "Bananen",
                "Birnen",
                "Butter",
                "Eier",
                "Frühstücksbeutel",
                "Geramond",
                "Gouda alt (Block)",
                "Gouda alt (Scheiben)",
                "Gouda jung (Block)",
                "Grillkohle",
                "Gurke",
                "Joghurt (klein)",
                "Joghurt (groß)",
                "Kiwi",
                "Kohlrabi",
                "Marmelade",
                "Milch",
                "Möhren",
                "Müllbeutel",
                "Müsliriegel",
                "Paprika",
                "Schinken",
                "Schinkenwurst (Scheiben)",
                "Schnittkäse",
                "Toilettenpapier",
                "Tücherbox",
                "Quark",
                "Weichkäse"
            };

            List<string> missing = preset.Except(connection.Table<Product>().Where(p => preset.Contains(p.Description)).Select(p => p.Description)).ToList();

            if (missing.Count > 0)
            {
                string sql = "select max(OrderId) as OrderId from Product";
                int maxOrderId = connection.ExecuteScalar<int>(sql);
                connection.InsertAll(missing.Select(m => new Product { Description = m, OrderId = ++maxOrderId }));
            }

            connection.CreateTable<ShoppingListProduct>();

            return new SQLiteAsyncConnection(() => connection);
        }

        #endregion
    }
}