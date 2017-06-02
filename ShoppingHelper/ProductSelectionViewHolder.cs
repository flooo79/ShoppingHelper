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
    public class ProductSelectionViewHolder : RecyclerView.ViewHolder
    {
        public TextView ProductNameTextView { get; private set; }
        public CheckBox SelectedCheckBox { get; private set; }

        public ProductSelectionViewHolder(View itemView) : base (itemView)
    {
            ProductNameTextView = itemView.FindViewById<TextView>(Resource.Id.ProductSelectionRowViewDescriptionTextView);
            SelectedCheckBox = itemView.FindViewById<CheckBox>(Resource.Id.ProductSelectionRowViewSelectionCheckBox);
        }   
    }
}