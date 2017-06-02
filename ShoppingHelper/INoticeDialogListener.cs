namespace ShoppingHelper
{
    using Android.App;

    public interface INoticeDialogListener
    {
        #region Public Methods and Operators

        void OnDialogNegativeClick(DialogFragment dialog);

        void OnDialogPositiveClick(DialogFragment dialog);

        #endregion
    }
}