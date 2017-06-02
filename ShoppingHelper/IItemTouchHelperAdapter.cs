namespace ShoppingHelper
{
    public interface IItemTouchHelperAdapter
    {
        #region Public Methods and Operators

        void OnItemDismiss(int position);

        void OnItemMove(int fromPosition, int toPosition);

        #endregion
    }
}