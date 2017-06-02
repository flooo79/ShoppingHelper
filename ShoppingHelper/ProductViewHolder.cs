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
            ProductNameTextView = itemView.FindViewById<TextView>(Resource.Id.ProductRowViewDescriptionTextView);
        }

        #endregion

        #region Public Properties

        public TextView ProductNameTextView { get; private set; }

        #endregion
    }
}