using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;

namespace ShoppingHelper
{
    public class ShoppingListAdapter: RecyclerView.Adapter
    {
        private List<ShoppingList> _shoppingLists;
        public event EventHandler<int> ItemClick;
            
        public ShoppingListAdapter(List<ShoppingList> shoppingLists)
        {
            _shoppingLists = shoppingLists;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ShoppingListRowView, parent, false);

            ShoppingListViewHolder viewHolder = new ShoppingListViewHolder(view, OnClick);            
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ShoppingListViewHolder viewHolder = holder as ShoppingListViewHolder;

            viewHolder.ShoppingListName.Text = _shoppingLists[position].Description;
            viewHolder.ItemCount.Text = string.Format("({0:N0})", _shoppingLists[position].Count);
        }

        public override int ItemCount
        {
            get { return _shoppingLists.Count; }
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}