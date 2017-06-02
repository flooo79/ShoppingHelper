using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using SQLite.Net;
using SQLite.Net.Async;

namespace ShoppingHelper.Model
{
    public static class ConnectionHelper
    {
        private static SQLiteConnection _connection;

        public static SQLiteConnection GetConnection()
        {
            SQLiteConnection connection =
                new SQLiteConnection(
                    new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(),
                    Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "shoppinghelper.db3"),
                    storeDateTimeAsTicks: false);

            connection.CreateTable<ShoppingList>();
            connection.CreateTable<Product>();
            connection.CreateTable<ShoppingListProduct>();
            return connection;
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
            connection.CreateTable<ShoppingListProduct>();

            return new SQLiteAsyncConnection(() => { return connection; });
        }

        public static void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}