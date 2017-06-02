namespace ShoppingHelper
{
    using System.Collections.Generic;

    using Android.Support.V7.Widget;
    using Android.Views;

    using ShoppingHelper.Model;

    public class ProductAdapter : RecyclerView.Adapter
    {
        #region Fields

        private List<Product> _products;

        #endregion

        #region Constructors and Destructors

        public ProductAdapter(List<Product> products)
        {
            _products = products;
        }

        #endregion

        #region Public Properties

        public override int ItemCount
        {
            get
            {
                return _products.Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductViewHolder viewHolder = holder as ProductViewHolder;
            viewHolder.ProductNameTextView.Text = _products[position].Description;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductRowView, parent, false);
            ProductViewHolder viewHolder = new ProductViewHolder(view);
            return viewHolder;
        }

        #endregion
    }
}