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
            ProductNameTextView = itemView.FindViewById<TextView>(Resource.Id.ProductSelectionRowViewDescriptionTextView);
            SelectedCheckBox = itemView.FindViewById<CheckBox>(Resource.Id.ProductSelectionRowViewSelectionCheckBox);
        }

        #endregion

        #region Public Properties

        public TextView ProductNameTextView { get; private set; }

        public CheckBox SelectedCheckBox { get; private set; }

        #endregion
    }
}