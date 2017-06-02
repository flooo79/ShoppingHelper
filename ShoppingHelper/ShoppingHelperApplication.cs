namespace ShoppingHelper
{
    using System;

    using Android.App;
    using Android.Runtime;

    using ShoppingHelper.Model;

    using SQLite.Net.Async;

    [Application(
#if DEBUG
        Debuggable = true,
#else
        Debuggable = false,
#endif
        Icon = "@drawable/ic_local_grocery_store_white_36dp",
        Label = "@string/ApplicationName",
        Theme = "@style/myTheme")]
    public class ShoppingHelperApplication : Application
    {
        private SQLiteAsyncConnection _connection;

        public SQLiteAsyncConnection Connection => _connection;

        public ShoppingHelperApplication(IntPtr handle, JniHandleOwnership ownerShip)
            : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _connection = ConnectionHelper.GetAsyncConnection();
        }

        public override void OnTerminate()
        {
            ConnectionHelper.CloseConnection();

            base.OnTerminate();
        }
    }
}