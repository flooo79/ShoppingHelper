using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using ShoppingHelper.Model;
using System.Collections.Generic;
using Android.Views;
using System;
using Android.Support.V4.App;

using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

using DialogFragment = Android.App.DialogFragment;
using SQLite.Net;
using System.Linq;
using Android.Support.V7.Widget.Helper;
using SQLite.Net.Async;
using System.Threading.Tasks;
using Android.Content;
using System.Threading;

namespace ShoppingHelper
{
    [Activity(Label = "StartShopping")]
    public class StartShoppingActivity : Activity, IItemTouchHelperAdapter
    {
        List<Product> _products;

        private RecyclerView _recyclerView;
        private RecyclerView.Adapter _adapter;
        private LinearLayoutManager _layoutManager;
        private SQLiteAsyncConnection _connection;
        private ShoppingList _shoppingList;
        private int _shoppingListId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _shoppingListId = Intent.GetIntExtra("ShoppingListId", 0);

            _products = new List<Product>();

            _connection = ((ShoppingHelperApplication)Application).Connection;            
                
            SetContentView(Resource.Layout.StartShopping);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.StartShoppingListTopToolbar);            
            SetActionBar(toolbar);

            ActionBar.Subtitle = GetString(Resource.String.StartShoppingToolbarSubtitle);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            _adapter = new ProductAdapter(_products);

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.StartShoppingRecyclerView);
            _recyclerView.SetAdapter(_adapter);
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);

            ItemTouchHelper.Callback itemTouchHelperCallback = new ProductItemTouchHelperCallback(this);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);
        }

        private void SelectProducts(int shoppingListId)
        {
            Intent intent = new Intent(this, typeof(ProductSelectionActivity));
            intent.PutExtra("ShoppingListId", shoppingListId);
            StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StartShoppingTopMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            if (id == Resource.Id.ProductSelectionMenuItem)
            {
                SelectProducts(_shoppingList.Id);
                return true;
            }
            
            return base.OnOptionsItemSelected(item);
        }

        public void OnItemDismiss(int position)
        {
            Product product = _products[position];
            _shoppingList.Products.Remove(product);
            
            _products.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            _connection.UpdateWithChildrenAsync(_shoppingList);
        }

        protected override void OnStart()
        {
            base.OnStart();

            string sql =
                "select Product.Id, Product.Description, Product.OrderId, case when ShoppingListProduct.ShoppingListId is null then 0 else 1 end as IsSelected " +
                "from Product " +
                "inner join ShoppingListProduct on Product.Id = ShoppingListProduct.ProductId and ShoppingListProduct.ShoppingListId = ? " +
                "order by Product.OrderId";

            _connection
                .GetAsync<ShoppingList>(_shoppingListId)
                .ContinueWith(t1 =>
                {
                    _shoppingList = t1.Result;
                    _shoppingList.Products = _connection.QueryAsync<Product>(sql, _shoppingListId).Result;
                    _products.Clear();
                    _products.AddRange(_shoppingList.Products);
                })
                .ContinueWith(t2 =>
                {
                    ActionBar.Title = _shoppingList.Description;
                    _adapter.NotifyDataSetChanged();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
        }
    }
}