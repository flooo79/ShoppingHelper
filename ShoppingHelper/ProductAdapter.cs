using Android.Support.V7.Widget;
using Android.Views;
using ShoppingHelper.Model;
using System.Collections.Generic;

namespace ShoppingHelper
{
    public class ProductAdapter : RecyclerView.Adapter
    {
        private List<Product> _products;

        public ProductAdapter(List<Product> products)
        {
            _products = products;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductRowView, parent, false);
            ProductViewHolder viewHolder = new ProductViewHolder(view);
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductViewHolder viewHolder = holder as ProductViewHolder;
            viewHolder.ProductNameTextView.Text = _products[position].Description;
        }

        public override int ItemCount
        {
            get { return _products.Count; }
        }        
    }
}