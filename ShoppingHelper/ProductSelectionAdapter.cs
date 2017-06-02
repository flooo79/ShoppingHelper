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
        #region Fields

        private List<Product> _products;

        #endregion

        #region Constructors and Destructors

        public ProductSelectionAdapter(List<Product> products)
        {
            _products = products;
        }

        #endregion

        #region Public Events

        public event EventHandler<Product> ProductDeselected;

        public event EventHandler<Product> ProductSelected;

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
            ProductSelectionViewHolder viewHolder = holder as ProductSelectionViewHolder;

            viewHolder.ProductNameTextView.Text = _products[position].Description;

            viewHolder.SelectedCheckBox.CheckedChange -= OnSelectedCheckBoxCheckedChange;

            viewHolder.SelectedCheckBox.Checked = _products[position].IsSelected;

            viewHolder.SelectedCheckBox.CheckedChange += OnSelectedCheckBoxCheckedChange;

            viewHolder.SelectedCheckBox.Tag = _products[position].Id;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ProductSelectionRowView, parent, false);
            ProductSelectionViewHolder viewHolder = new ProductSelectionViewHolder(view);
            viewHolder.SelectedCheckBox.CheckedChange += OnSelectedCheckBoxCheckedChange;
            return viewHolder;
        }

        #endregion

        #region Methods

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

        #endregion
    }
}