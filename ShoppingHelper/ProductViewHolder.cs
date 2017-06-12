namespace ShoppingHelper
{
    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;

    public class ProductViewHolder : RecyclerView.ViewHolder
    {
        #region Constructors and Destructors

        public ProductViewHolder(View itemView)
            : base(itemView)
        {
            ProductQuantityTextView = itemView.FindViewById<TextView>(Resource.Id.start_shopping_product_quantity_textview);
            ProductDescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.start_shopping_product_description_textview);
        }

        #endregion

        #region Public Properties

        public TextView ProductDescriptionTextView { get; }

        public TextView ProductQuantityTextView { get; }

        #endregion
    }
}