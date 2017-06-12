namespace ShoppingHelper
{
    using System.Collections.Generic;

    using Android.Support.V7.Widget;
    using Android.Views;

    using ShoppingHelper.Model;

    public class ProductAdapter : RecyclerView.Adapter
    {
        #region Fields

        private readonly List<ShoppingListProduct> _shoppingListProducts;

        #endregion

        #region Constructors and Destructors

        public ProductAdapter(List<ShoppingListProduct> shoppingListProducts)
        {
            _shoppingListProducts = shoppingListProducts;
        }

        #endregion

        #region Public Properties

        public override int ItemCount => _shoppingListProducts.Count;

        #endregion

        #region Public Methods and Operators

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductViewHolder viewHolder = holder as ProductViewHolder;

            if (viewHolder == null)

            {
                return;
            }

            viewHolder.ProductQuantityTextView.Text = _shoppingListProducts[position].Quantity > 1 ? $"{_shoppingListProducts[position].Quantity:0'x}" : null;
            viewHolder.ProductDescriptionTextView.Text = _shoppingListProducts[position].Product?.Description;
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