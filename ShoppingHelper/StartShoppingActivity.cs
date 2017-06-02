namespace ShoppingHelper
{
    using System.Collections.Generic;
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

    [Activity(Label = "StartShopping")]
    public class StartShoppingActivity : Activity, IItemTouchHelperAdapter
    {
        #region Fields

        private RecyclerView.Adapter _adapter;

        private SQLiteAsyncConnection _connection;

        private LinearLayoutManager _layoutManager;

        private List<Product> _products;

        private RecyclerView _recyclerView;

        private ShoppingList _shoppingList;

        private int _shoppingListId;

        #endregion

        #region Public Methods and Operators

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StartShoppingTopMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnItemDismiss(int position)
        {
            Product product = _products[position];
            _shoppingList.Products.Remove(product);

            _products.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            _connection.UpdateWithChildrenAsync(_shoppingList);
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

            if (id == Resource.Id.ProductSelectionMenuItem)
            {
                SelectProducts(_shoppingList.Id);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Methods

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
                .ContinueWith(
                    t1 =>
                    {
                        _shoppingList = t1.Result;
                        _shoppingList.Products = _connection.QueryAsync<Product>(sql, _shoppingListId).Result;
                        _products.Clear();
                        _products.AddRange(_shoppingList.Products);
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