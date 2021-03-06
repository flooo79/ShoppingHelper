﻿namespace ShoppingHelper
{
    using System;
    using System.Collections.Generic;

    using Android.Support.V7.Widget;
    using Android.Views;

    using ShoppingHelper.Model;

    public class ShoppingListAdapter : RecyclerView.Adapter
    {
        #region Fields

        private readonly List<ShoppingList> _shoppingLists;

        #endregion

        #region Constructors and Destructors

        public ShoppingListAdapter(List<ShoppingList> shoppingLists)
        {
            _shoppingLists = shoppingLists;
        }

        #endregion

        #region Public Events

        public event EventHandler<int> ItemClick;

        #endregion

        #region Public Properties

        public override int ItemCount => _shoppingLists.Count;

        #endregion

        #region Public Methods and Operators

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ShoppingListViewHolder viewHolder = holder as ShoppingListViewHolder;

            if (viewHolder == null)
            {
                return;
            }

            viewHolder.ShoppingListName.Text = _shoppingLists[position].Description;
            viewHolder.ItemCount.Text = $"({_shoppingLists[position].Products.Count:N0})";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ShoppingListRowView, parent, false);

            ShoppingListViewHolder viewHolder = new ShoppingListViewHolder(view, OnClick);
            return viewHolder;
        }

        #endregion

        #region Methods

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        #endregion
    }
}