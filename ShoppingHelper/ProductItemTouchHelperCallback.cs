namespace ShoppingHelper
{
    using Android.Support.V7.Widget;
    using Android.Support.V7.Widget.Helper;

    public class ProductItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        #region Fields

        private IItemTouchHelperAdapter _itemTouchHelperAdapter;

        #endregion

        #region Constructors and Destructors

        public ProductItemTouchHelperCallback(IItemTouchHelperAdapter itemTouchHelperAdaptter)
        {
            _itemTouchHelperAdapter = itemTouchHelperAdaptter;
        }

        #endregion

        #region Public Properties

        public override bool IsItemViewSwipeEnabled => true;

        public override bool IsLongPressDragEnabled => false;

        #endregion

        #region Public Methods and Operators

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            _itemTouchHelperAdapter.OnItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            _itemTouchHelperAdapter.OnItemDismiss(viewHolder.AdapterPosition);
        }

        #endregion
    }
}