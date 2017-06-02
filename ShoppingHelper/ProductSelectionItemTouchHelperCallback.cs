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
using Android.Support.V7.Widget.Helper;
using Android.Support.V7.Widget;

namespace ShoppingHelper
{
    public class ProductSelectionItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private IItemTouchHelperAdapter _itemTouchHelperAdapter;

        public ProductSelectionItemTouchHelperCallback(IItemTouchHelperAdapter itemTouchHelperAdaptter)
        {
            _itemTouchHelperAdapter = itemTouchHelperAdaptter;
        }

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

        public override bool IsItemViewSwipeEnabled => true;

        public override bool IsLongPressDragEnabled => true;
    }
}