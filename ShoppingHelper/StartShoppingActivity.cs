namespace ShoppingHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V7.Widget;
    using Android.Support.V7.Widget.Helper;
    using Android.Views;
    using Android.Widget;

    using ShoppingHelper.Model;

    using SQLite.Net.Async;

    using SQLiteNetExtensionsAsync.Extensions;

    using Environment = System.Environment;

    [Activity(Label = "StartShopping")]
    public class StartShoppingActivity : Activity, IItemTouchHelperAdapter
    {
        #region Fields

        private ProductAdapter _adapter;

        private SQLiteAsyncConnection _connection;

        private LinearLayoutManager _layoutManager;

        private RecyclerView _recyclerView;

        private ShareActionProvider _shareActionProvider;

        private ShoppingList _shoppingList;

        private int _shoppingListId;

        private List<ShoppingListProduct> _shoppingListProducts;

        #endregion

        #region Public Methods and Operators

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StartShoppingTopMenu, menu);

            // Locate MenuItem with ShareActionProvider
            IMenuItem item = menu.FindItem(Resource.Id.start_shopping_menu_share);

            // Fetch and store ShareActionProvider
            _shareActionProvider = (ShareActionProvider)item.ActionProvider;

            // Return true to display menu
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnItemDismiss(int position)
        {
            int shoppingListProductiD = _shoppingListProducts[position].Id;
            _shoppingListProducts.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            ShoppingListProduct shoppingListProduct = _shoppingList.Products.FirstOrDefault(s => s.Id == shoppingListProductiD);

            if (shoppingListProduct == null)
            {
                return;
            }

            _shoppingList.Products.Remove(shoppingListProduct);

            _connection.DeleteAsync<ShoppingListProduct>(shoppingListProduct.Id)
                .ContinueWith(
                    t =>
                    {
                        _shoppingList.LastUpdated = DateTime.Now;
                        _connection.UpdateAsync(_shoppingList).Wait();
                    });
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            if (id == Resource.Id.start_shopping_menu_edit)
            {
                SelectProducts(_shoppingList.Id);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            if (_shoppingList == null)
            {
                return true;
            }

            Intent intent = new Intent();
            intent.SetAction(Intent.ActionSend);

            string extraText = string.Join(Environment.NewLine, _shoppingList.Products.Select(p => $"{p.Quantity}x {p.Product?.Description ?? string.Empty}"));
            intent.PutExtra(Intent.ExtraText, extraText);

            intent.PutExtra(Intent.ExtraSubject, _shoppingList.Description);
            intent.SetType("text/plain");
            _shareActionProvider.SetShareIntent(intent);

            return base.OnPrepareOptionsMenu(menu);
        }

        #endregion

        #region Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _shoppingListId = Intent.GetIntExtra("ShoppingListId", 0);

            _connection = ((ShoppingHelperApplication)Application).Connection;

            SetContentView(Resource.Layout.StartShopping);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.StartShoppingListTopToolbar);
            SetActionBar(toolbar);

            ActionBar.Subtitle = GetString(Resource.String.StartShoppingToolbarSubtitle);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            _shoppingListProducts = new List<ShoppingListProduct>();
            _adapter = new ProductAdapter(_shoppingListProducts);

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.StartShoppingRecyclerView);
            _recyclerView.SetAdapter(_adapter);

            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);

            ItemTouchHelper.Callback itemTouchHelperCallback = new ProductItemTouchHelperCallback(this);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _connection
                .GetWithChildrenAsync<ShoppingList>(_shoppingListId, true)
                .ContinueWith(
                    t1 =>
                    {
                        _shoppingList = t1.Result;
                        _shoppingListProducts.Clear();
                        _shoppingListProducts.AddRange(_shoppingList.Products.OrderBy(p => p.Product.OrderId));
                    })
                .ContinueWith(
                    t2 =>
                    {
                        ActionBar.Title = _shoppingList.Description;
                        _adapter.NotifyDataSetChanged();
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SelectProducts(int shoppingListId)
        {
            Intent intent = new Intent(this, typeof(ProductSelectionActivity));
            intent.PutExtra("ShoppingListId", shoppingListId);
            StartActivity(intent);
        }

        #endregion
    }
}