namespace ShoppingHelper
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Android.App;
    using Android.OS;
    using Android.Support.V7.Widget;
    using Android.Support.V7.Widget.Helper;
    using Android.Views;
    using Android.Widget;

    using ShoppingHelper.Model;

    using SQLite.Net.Async;

    using SQLiteNetExtensionsAsync.Extensions;

    [Activity(Label = "SelectProducts")]
    public class ProductSelectionActivity : Activity, INoticeDialogListener, IItemTouchHelperAdapter
    {
        #region Fields

        private ProductSelectionAdapter _adapter;

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
            MenuInflater.Inflate(Resource.Menu.EditShoppingListTopMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnDialogNegativeClick(DialogFragment dialog)
        {
        }

        public void OnDialogPositiveClick(DialogFragment dialog)
        {
            EditText editText = (EditText)dialog.Dialog.CurrentFocus;
            string productName = string.IsNullOrEmpty(editText.Text) ? "<no name>" : editText.Text;

            int position = 0;

            Product product = _connection.Table<Product>().Where(p => productName.ToLower() == p.Description.ToLower()).FirstOrDefaultAsync().Result;

            if (product == null)
            {
                string sql = "select max(OrderId) as OrderId from Product";
                int orderId = _connection.ExecuteScalarAsync<int>(sql).Result;

                product = new Product { Description = productName, IsSelected = true, OrderId = ++orderId };
                product.ShoppingLists.Add(_shoppingList);
                _products.Insert(_products.Count, product);
                _adapter.NotifyItemInserted(_products.Count);
                position = _products.Count - 1;
                _shoppingList.LastUpdated = DateTime.Now;

                _connection
                    .InsertAsync(product)
                    .ContinueWith(
                        t1 => _connection.UpdateWithChildrenAsync(product)
                            .ContinueWith(t2 => _connection.UpdateAsync(_shoppingList)));
            }
            else
            {
                for (int i = 0; i < _products.Count; i++)
                {
                    if (string.Equals(_products[i].Description, productName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        position = i;
                        product = _products[i];
                        product.IsSelected = true;

                        if (!product.ShoppingLists.Contains(_shoppingList))
                        {
                            product.ShoppingLists.Add(_shoppingList);
                        }

                        _adapter.NotifyItemChanged(position);

                        _shoppingList.LastUpdated = DateTime.Now;
                        _connection.UpdateWithChildrenAsync(product).ContinueWith(t => _connection.UpdateAsync(_shoppingList));
                        break;
                    }
                }
            }

            _recyclerView.ScrollToPosition(position);
        }

        public void OnItemDismiss(int position)
        {
            Product product = _products[position];
            product.ShoppingLists.Clear();
            _products.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            _connection.UpdateWithChildrenAsync(product).ContinueWith(t => _connection.DeleteAsync(product));
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    int fromOrderId = _products[i].OrderId;
                    int toOrderId = _products[i + 1].OrderId;

                    Product tmp = _products[i];
                    _products[i] = _products[i + 1];
                    _products[i + 1] = tmp;

                    _products[i].OrderId = fromOrderId;
                    _products[i + 1].OrderId = toOrderId;

                    _connection.UpdateAllAsync(new[] { _products[i], _products[i + 1] });
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    int fromOrderId = _products[i].OrderId;
                    int toOrderId = _products[i - 1].OrderId;

                    Product tmp = _products[i];
                    _products[i] = _products[i - 1];
                    _products[i - 1] = tmp;

                    _products[i].OrderId = fromOrderId;
                    _products[i - 1].OrderId = toOrderId;

                    _connection.UpdateAllAsync(new[] { _products[i], _products[i - 1] });
                }
            }

            _adapter.NotifyItemMoved(fromPosition, toPosition);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            if (id == Resource.Id.AddProductMenuItem)
            {
                new AddProductDialogFragment().Show(FragmentManager, "AddProductDialog");
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

            SetContentView(Resource.Layout.ProductSelection);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.ProductSelectionTopToolbar);
            SetActionBar(toolbar);
            ActionBar.Title = GetString(Resource.String.ProductSelection);
            ActionBar.Subtitle = GetString(Resource.String.ProductSelectionToolbarSubtitle);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            _adapter = new ProductSelectionAdapter(_products);
            _adapter.ProductSelected += OnAdapterProductSelected;
            _adapter.ProductDeselected += OnAdapterProductDeselected;

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.ProductSelectionRecyclerView);
            _recyclerView.SetAdapter(_adapter);
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);

            ItemTouchHelper.Callback itemTouchHelperCallback = new ProductSelectionItemTouchHelperCallback(this);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _connection
                .QueryAsync<Product>(
                    "select Product.Id, Product.Description, Product.OrderId, case when ShoppingListProduct.ShoppingListId is null then 0 else 1 end as IsSelected " +
                    "from Product " +
                    "left join ShoppingListProduct on Product.Id = ShoppingListProduct.ProductId and ShoppingListProduct.ShoppingListId = ? " +
                    "order by Product.OrderId",
                    _shoppingListId)
                .ContinueWith(
                    r =>
                    {
                        _products.Clear();
                        _products.AddRange(r.Result);
                    })
                .ContinueWith(r => _adapter.NotifyDataSetChanged(), TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(r => { _shoppingList = _connection.GetWithChildrenAsync<ShoppingList>(_shoppingListId).Result; });
        }

        private void OnAdapterProductDeselected(object sender, Product product)
        {
            product.ShoppingLists.Remove(_shoppingList);

            _shoppingList.Products.Remove(product);
            _shoppingList.LastUpdated = DateTime.Now;

            _connection.UpdateWithChildrenAsync(product);
            _connection.UpdateAsync(_shoppingList);
        }

        private void OnAdapterProductSelected(object sender, Product product)
        {
            product.ShoppingLists.Add(_shoppingList);

            if (!_shoppingList.Products.Contains(product))
            {
                _shoppingList.Products.Add(product);
            }

            _shoppingList.LastUpdated = DateTime.Now;

            _connection.UpdateWithChildrenAsync(_shoppingList);
        }

        #endregion
    }
}