namespace ShoppingHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;

    using ShoppingHelper.Model;

    public class ProductSelectionAdapter : RecyclerView.Adapter
    {
        #region Constants

        private const int MaxValue = 9;

        private const int MinValue = 0;

        #endregion

        #region Fields

        private readonly List<ProductQuantity> _productQuantities;

        #endregion

        #region Constructors and Destructors

        public ProductSelectionAdapter(List<ProductQuantity> productQuantities)
        {
            _productQuantities = productQuantities;
        }

        #endregion

        #region Public Events

        public event EventHandler<ProductQuantity> QuantityChanged;

        #endregion

        #region Public Properties

        public override int ItemCount => _productQuantities.Count;

        #endregion

        #region Public Methods and Operators

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ProductSelectionViewHolder viewHolder = holder as ProductSelectionViewHolder;

            if (viewHolder == null)
            {
                return;
            }

            viewHolder.DescriptionTextView.Text = _productQuantities[position].Product.Description;
            viewHolder.QuantityTextView.Text = _productQuantities[position].Quantity.ToString("0");
            viewHolder.LessButton.Enabled = _productQuantities[position].Quantity > MinValue;
            viewHolder.LessButton.Tag = _productQuantities[position].Product.Id;
            viewHolder.MoreButton.Enabled = _productQuantities[position].Quantity < MaxValue;
            viewHolder.MoreButton.Tag = _productQuantities[position].Product.Id;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductSelectionRowView, parent, false);
            ProductSelectionViewHolder viewHolder = new ProductSelectionViewHolder(view);
            viewHolder.LessButton.Click += OnLessButtonClick;
            viewHolder.MoreButton.Click += OnMoreButtonClick;
            return viewHolder;
        }

        #endregion

        #region Methods

        private void ChangeProductQuantity(int productId, int diffAmount)
        {
            ProductQuantity productQuantity = _productQuantities.FirstOrDefault(p => p.Product.Id == productId);

            if (productQuantity == null)
            {
                return;
            }

            productQuantity.Quantity = productQuantity.Quantity + diffAmount;

            QuantityChanged?.Invoke(this, productQuantity);
        }

        private void OnLessButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            int productId = (int)button.Tag;

            ChangeProductQuantity(productId, -1);
        }

        private void OnMoreButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            int productId = (int)button.Tag;

            ChangeProductQuantity(productId, +1);
        }

        #endregion
    }
}