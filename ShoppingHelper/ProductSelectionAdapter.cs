using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ShoppingHelper.Model;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingHelper
{
    public class ProductSelectionAdapter : RecyclerView.Adapter
    {
        public event EventHandler<Product> ProductSelected;
        public event EventHandler<Product> ProductDeselected;

        private List<Product> _products;

        public ProductSelectionAdapter(List<Product> products)
        {
            _products = products;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductSelectionRowView, parent, false);
            ProductSelectionViewHolder viewHolder = new ProductSelectionViewHolder(view);
            viewHolder.SelectedCheckBox.CheckedChange += OnSelectedCheckBoxCheckedChange;
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductSelectionViewHolder viewHolder = holder as ProductSelectionViewHolder;

            viewHolder.ProductNameTextView.Text = _products[position].Description;

            viewHolder.SelectedCheckBox.CheckedChange -= OnSelectedCheckBoxCheckedChange;

            viewHolder.SelectedCheckBox.Checked = _products[position].IsSelected;

            viewHolder.SelectedCheckBox.CheckedChange += OnSelectedCheckBoxCheckedChange;
            
            viewHolder.SelectedCheckBox.Tag = _products[position].Id;
        }

        private void OnProductDeselected(Product product)
        {
            ProductDeselected?.Invoke(this, product);
        }

        private void OnProductSelected(Product product)
        {
            ProductSelected?.Invoke(this, product);
        }

        private void OnSelectedCheckBoxCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox == null)
            {
                return;
            }

            int productId = (int)checkBox.Tag;

            Product product = _products.First(p => p.Id == productId);            

            if (checkBox.Checked)
            {
                OnProductSelected(product);

            }
            else
            {
                OnProductDeselected(product);
            }
        }

        public override int ItemCount
        {
            get { return _products.Count; }
        }        
    }
}