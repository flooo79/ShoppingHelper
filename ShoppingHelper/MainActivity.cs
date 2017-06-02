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

    [Activity(Label = "ShoppingHelper", MainLauncher = true, Icon = "@drawable/ic_local_grocery_store_white_36dp")]
    public class MainActivity : Activity, INoticeDialogListener, IItemTouchHelperAdapter
    {
        #region Fields

        private ShoppingListAdapter _adapter;

        private SQLiteAsyncConnection _connection;

        private RecyclerView _recyclerView;

        private List<ShoppingList> _shoppingLists;

        #endregion

        #region Public Methods and Operators

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ShoppingListTopMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public void OnDialogNegativeClick(DialogFragment dialog)
        {
            dialog.Dismiss();
        }

        public void OnDialogPositiveClick(DialogFragment dialog)
        {
            EditText editText = (EditText)dialog.Dialog.CurrentFocus;
            string shoppingListName = string.IsNullOrEmpty(editText.Text) ? "<no name>" : editText.Text;

            ShoppingList shoppingList = new ShoppingList { Description = shoppingListName };
            _shoppingLists.Insert(0, shoppingList);
            _adapter.NotifyItemInserted(0);
            _recyclerView.ScrollToPosition(0);

            _connection
                .InsertAsync(shoppingList)
                .ContinueWith(t => SelectProducts(shoppingList.Id), TaskScheduler.FromCurrentSynchronizationContext());

            dialog.Dismiss();
        }

        public void OnItemDismiss(int position)
        {
            ShoppingList shoppingList = _shoppingLists[position];
            _shoppingLists.RemoveAt(position);
            _adapter.NotifyItemRemoved(position);

            shoppingList.Products.Clear();
            _connection.UpdateWithChildrenAsync(shoppingList).ContinueWith(t => _connection.DeleteAsync(shoppingList));
        }

        public void OnItemMove(int fromPosition, int toPosition)
        {
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    ShoppingList tmp = _shoppingLists[i];
                    _shoppingLists[i] = _shoppingLists[i + 1];
                    _shoppingLists[i + 1] = tmp;
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    ShoppingList tmp = _shoppingLists[i];
                    _shoppingLists[i] = _shoppingLists[i - 1];
                    _shoppingLists[i - 1] = tmp;
                }
            }

            _adapter.NotifyItemMoved(fromPosition, toPosition);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            new AddShoppingListDialogFragment().Show(FragmentManager, "AddShoppingListDialog");

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _shoppingLists = new List<ShoppingList>();

            _connection = ((ShoppingHelperApplication)Application).Connection;

            //Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.ShoppingListTopToolbar);
            SetActionBar(toolbar);
            ActionBar.Title = GetString(Resource.String.ShoppingLists);
            ActionBar.Subtitle = GetString(Resource.String.ShoppingListsToolbarSubtitle);

            _adapter = new ShoppingListAdapter(_shoppingLists);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.ShoppingListRecyclerView);
            _recyclerView.SetAdapter(_adapter);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            ItemTouchHelper.Callback itemTouchHelperCallback = new ShoppingListItemTouchHelperCallback(this);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(itemTouchHelperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            _adapter.ItemClick += OnAdapterItemClick;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected override void OnStart()
        {
            base.OnStart();

            string sql =
                "select ShoppingList.Id, ShoppingList.Description, count(ShoppingListProduct.ProductId) as Count " +
                "from ShoppingList " +
                "left join ShoppingListProduct on ShoppingList.Id = ShoppingListProduct.ShoppingListId " +
                "group by ShoppingList.Id, ShoppingList.Description " +
                "order by ShoppingList.LastUpdated desc";

            _connection
                .QueryAsync<ShoppingList>(sql)
                .ContinueWith(
                    t =>
                    {
                        _shoppingLists.Clear();
                        _shoppingLists.AddRange(t.Result);
                    })
                .ContinueWith(t => _adapter.NotifyDataSetChanged(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnStop()
        {
            _adapter.ItemClick -= OnAdapterItemClick;

            base.OnStop();
        }

        private void OnAdapterItemClick(object sender, int position)
        {
            StartShopping(_shoppingLists[position].Id);
        }

        private void SelectProducts(int shoppingListId)
        {
            Intent intent = new Intent(this, typeof(ProductSelectionActivity));
            intent.PutExtra("ShoppingListId", shoppingListId);
            StartActivity(intent);
        }

        private void StartShopping(int shoppingListId)
        {
            Intent intent = new Intent(this, typeof(StartShoppingActivity));
            intent.PutExtra("ShoppingListId", shoppingListId);
            StartActivity(intent);
        }

        #endregion
    }
}