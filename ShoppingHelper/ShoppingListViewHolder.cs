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
using Android.Support.V7.Widget;

namespace ShoppingHelper
{
    public class ShoppingListViewHolder : RecyclerView.ViewHolder
    {
        public TextView ShoppingListName { get; private set; }
        public TextView ItemCount { get; private set; }

        public ShoppingListViewHolder(View itemView, Action<int> listener) : base (itemView)
    {
            ShoppingListName = itemView.FindViewById<TextView>(Resource.Id.ShoppingListRowViewDescriptionTextView);
            ItemCount = itemView.FindViewById<TextView>(Resource.Id.ShoppingListRowViewCountTextView);
            
            itemView.Click += (sender, e) => listener(AdapterPosition);
        }   
    }
}