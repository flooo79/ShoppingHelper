namespace ShoppingHelper.Model
{
    using System.IO;

    using Android.Content.PM;

    using SQLite.Net;
    using SQLite.Net.Async;

    public static class ConnectionHelper
    {
        #region Static Fields

        private static int VersionCode = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, 0).VersionCode;

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
                    new SQLiteConnectionString(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "shoppinghelper.db3"), storeDateTimeAsTicks: false));

            connection.CreateTable<ShoppingList>();
            connection.CreateTable<Product>();
            connection.CreateTable<ShoppingListProduct>();

            return new SQLiteAsyncConnection(() => connection);
        }

        #endregion
    }
}