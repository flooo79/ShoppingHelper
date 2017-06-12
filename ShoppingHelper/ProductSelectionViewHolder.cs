namespace ShoppingHelper
{
    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;

    public class ProductSelectionViewHolder : RecyclerView.ViewHolder
    {
        #region Constructors and Destructors

        public ProductSelectionViewHolder(View itemView)
            : base(itemView)
        {
            DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.product_selection_description_textview);
            LessButton = itemView.FindViewById<Button>(Resource.Id.product_selection_less_button);
            QuantityTextView = itemView.FindViewById<TextView>(Resource.Id.product_selection_quantity_textview);
            MoreButton = itemView.FindViewById<Button>(Resource.Id.product_selection_more_button);
        }

        #endregion

        #region Public Properties

        public TextView DescriptionTextView { get; }

        public Button LessButton { get; }

        public Button MoreButton { get; }

        public TextView QuantityTextView { get; }

        #endregion
    }
}