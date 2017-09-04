namespace ShoppingHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private List<ProductQuantity> _productQuantities;

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

                _connection.ExecuteScalarAsync<int>(sql)
                    .ContinueWith(
                        t1 =>
                        {
                            int orderId = t1.Result;
                            product = new Product { Description = productName, OrderId = ++orderId };
                            _connection.InsertAsync(product).Wait();
                            return product;
                        })
                    .ContinueWith(
                        t2 =>
                        {
                            ProductQuantity productQuantity = new ProductQuantity { Product = t2.Result, Quantity = 1 };
                            _productQuantities.Add(productQuantity);
                            position = _productQuantities.Count - 1;
                            _adapter.NotifyItemInserted(position);
                            _recyclerView.ScrollToPosition(position);
                            return productQuantity;
                        },
                        TaskScheduler.FromCurrentSynchronizationContext())
                    .ContinueWith(
                        t3 =>
                        {
                            ProductQuantity productQuantity = t3.Result;
                            ShoppingListProduct shoppingListProduct = new ShoppingListProduct { ShoppingListId = _shoppingListId, ProductId = productQuantity.Product.Id, Product = productQuantity.Product, Quantity = productQuantity.Quantity };
                            _connection.InsertWithChildrenAsync(shoppingListProduct, true).Wait();
                            _shoppingList.Products.Add(shoppingListProduct);
                            _shoppingList.LastUpdated = DateTime.Now;
                            _connection.UpdateAsync(_shoppingList).Wait();
                        });
            }
            else
            {
                for (int i = 0; i < _productQuantities.Count; i++)
                {
                    if (string.Equals(_productQuantities[i].Product.Description, productName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        position = i;

                        ProductQuantity productQuantity = _productQuantities[i];

                        if (productQuantity.Quantity == 0)
                        {
                            productQuantity.Quantity = 1;
                        }

                        _adapter.NotifyItemChanged(position);
                        _recyclerView.ScrollToPosition(position);

                        Task task = new Task(
                            () =>
                            {
                                ShoppingListProduct shoppingListProduct = _shoppingList.Products.FirstOrDefault(p => p.Product.Id == productQuantity.Product.Id);

                                if (shoppingListProduct == null)
                                {
                                    shoppingListProduct = new ShoppingListProduct { ShoppingListId = _shoppingListId, ProductId = productQuantity.Product.Id, Product = productQuantity.Product, Quantity = productQuantity.Quantity };
                                    _connection.InsertWithChildrenAsync(shoppingListProduct, true).Wait();
                                    _shoppingList.Products.Add(shoppingListProduct);
                                }
                                else
                                {
                                    shoppingListProduct.Quantity = productQuantity.Quantity;
                                    _connection.UpdateAsync(shoppingListProduct).Wait();
                                }

                                _shoppingList.LastUpdated = DateTime.Now;
                                _connection.UpdateAsync(_shoppingList).Wait();
                            }
                        );

                        task.Start();
                        break;
                    }
                }
            }
        }

        public void OnItemDismiss(int position)
        {
            ProductQuantity productQuantity = _productQuantities[position];
            _productQuantities.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            ShoppingListProduct shoppingListProduct = _shoppingList.Products.FirstOrDefault(p => p.ProductId == productQuantity.Product.Id);

            if (shoppingListProduct != null)
            {
                _shoppingList.Products.Remove(shoppingListProduct);
                Task task1 = _connection.DeleteAsync(shoppingListProduct);
                _shoppingList.LastUpdated = DateTime.Now;
                Task task2 = _connection.UpdateAsync(_shoppingList);
                Task.WaitAll(task1, task2);
            }

            _connection.DeleteAsync(productQuantity.Product);
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    int fromOrderId = _productQuantities[i].Product.OrderId;
                    int toOrderId = _productQuantities[i + 1].Product.OrderId;

                    Product tmp = _productQuantities[i].Product;
                    _productQuantities[i].Product = _productQuantities[i + 1].Product;
                    _productQuantities[i + 1].Product = tmp;

                    _productQuantities[i].Product.OrderId = fromOrderId;
                    _productQuantities[i + 1].Product.OrderId = toOrderId;

                    _connection.UpdateAllAsync(new[] { _productQuantities[i].Product, _productQuantities[i + 1].Product });
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    int fromOrderId = _productQuantities[i].Product.OrderId;
                    int toOrderId = _productQuantities[i - 1].Product.OrderId;

                    Product tmp = _productQuantities[i].Product;
                    _productQuantities[i].Product = _productQuantities[i - 1].Product;
                    _productQuantities[i - 1].Product = tmp;

                    _productQuantities[i].Product.OrderId = fromOrderId;
                    _productQuantities[i - 1].Product.OrderId = toOrderId;

                    _connection.UpdateAllAsync(new[] { _productQuantities[i].Product, _productQuantities[i - 1].Product });
                }
            }

            _adapter.NotifyItemMoved(fromPosition, toPosition);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            if (item.ItemId == Resource.Id.AddProductMenuItem)
            {
                new AddProductDialogFragment().Show(FragmentManager, "AddProductDialog");
                return true;
            }

            if (item.ItemId == Resource.Id.SortByNameMenuItem)
            {
                SortByName();
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

            _connection = ((ShoppingHelperApplication)Application).Connection;

            SetContentView(Resource.Layout.ProductSelection);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.ProductSelectionTopToolbar);
            SetActionBar(toolbar);
            ActionBar.Title = GetString(Resource.String.ProductSelection);
            ActionBar.Subtitle = GetString(Resource.String.ProductSelectionToolbarSubtitle);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.ProductSelectionRecyclerView);
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);

            _productQuantities = new List<ProductQuantity>();

            _adapter = new ProductSelectionAdapter(_productQuantities);
            _adapter.QuantityChanged += OnProductQuantityChanged;
            _recyclerView.SetAdapter(_adapter);

            ItemTouchHelper.Callback itemTouchHelperCallback = new ProductSelectionItemTouchHelperCallback(this);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);
        }

        protected override void OnStart()
        {
            base.OnStart();

            LoadData();
        }

        private void LoadData()
        {
            Task task1 =
                _connection.GetWithChildrenAsync<ShoppingList>(_shoppingListId, recursive: true)
                    .ContinueWith(
                        t1 => { _shoppingList = t1.Result; });

            Task task2 =
                _connection.Table<Product>().OrderBy(p => p.OrderId).ToListAsync()
                    .ContinueWith(
                        t2 =>
                        {
                            _productQuantities.Clear();
                            t2.Result.ForEach(p => _productQuantities.Add(new ProductQuantity { Product = p, Quantity = 0 }));
                        });

            Task.WaitAll(task1, task2);

            foreach (ShoppingListProduct shoppingListProduct in _shoppingList.Products)
            {
                ProductQuantity productQuantity = _productQuantities.FirstOrDefault(p => p.Product.Equals(shoppingListProduct.Product));

                if (productQuantity != null)
                {
                    productQuantity.Quantity = shoppingListProduct.Quantity;
                }
            }

            _adapter.NotifyDataSetChanged();
        }

        private void OnProductQuantityChanged(object sender, ProductQuantity productQuantity)
        {
            if (productQuantity?.Product == null)
            {
                return;
            }

            Task task;

            ShoppingListProduct shoppingListProduct = _shoppingList.Products.FirstOrDefault(p => p.ProductId == productQuantity.Product.Id);

            if (shoppingListProduct == null)
            {
                shoppingListProduct = new ShoppingListProduct { ShoppingListId = _shoppingListId, ProductId = productQuantity.Product.Id, Product = productQuantity.Product, Quantity = productQuantity.Quantity };
                task = _connection.InsertWithChildrenAsync(shoppingListProduct, true).ContinueWith(t => _shoppingList.Products.Add(shoppingListProduct));
            }
            else
            {
                shoppingListProduct.Quantity = productQuantity.Quantity;
                task = _connection.UpdateAsync(shoppingListProduct);
            }

            int index = _productQuantities.IndexOf(productQuantity);

            _adapter.NotifyItemChanged(index);

            task.ContinueWith(
                t =>
                {
                    _shoppingList.LastUpdated = DateTime.Now;
                    _connection.UpdateAsync(_shoppingList);
                });
        }

        private void SortByName()
        {
            Task task = _connection.Table<Product>().OrderBy(p => p.Description).ToListAsync()
                .ContinueWith(
                    t1 =>
                    {
                        int orderId = 0;
                        t1.Result.ForEach(p => p.OrderId = ++orderId);
                        _connection.UpdateAllAsync(t1.Result).Wait();
                    });

            Task.WaitAll(task);

            LoadData();
        }

        #endregion
    }
}