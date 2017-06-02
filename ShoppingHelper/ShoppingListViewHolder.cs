namespace ShoppingHelper
{
    using System;

    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;

    public class ShoppingListViewHolder : RecyclerView.ViewHolder
    {
        #region Constructors and Destructors

        public ShoppingListViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            ShoppingListName = itemView.FindViewById<TextView>(Resource.Id.ShoppingListRowViewDescriptionTextView);
            ItemCount = itemView.FindViewById<TextView>(Resource.Id.ShoppingListRowViewCountTextView);

            itemView.Click += (sender, e) => listener(AdapterPosition);
        }

        #endregion

        #region Public Properties

        public TextView ItemCount { get; private set; }

        public TextView ShoppingListName { get; private set; }

        #endregion
    }
}