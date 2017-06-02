namespace ShoppingHelper
{
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

    using Java.Lang;

    public class AddProductDialogFragment : DialogFragment
    {
        #region Fields

        private INoticeDialogListener _noticeDialogListener;

        #endregion

        #region Public Methods and Operators

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);

            try
            {
                _noticeDialogListener = (INoticeDialogListener)activity;
            }
            catch (ClassCastException)
            {
                throw new ClassCastException(activity + " must implement INoticeDialogListener");
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            View view = Activity.LayoutInflater.Inflate(Resource.Layout.AddProduct, null);

            builder.SetTitle(GetString(Resource.String.ProductName));
            builder.SetView(view);

            EditText editText = view.FindViewById<EditText>(Resource.Id.AddProductNameEditText);

            editText.RequestFocus();
            editText.SetTextColor(Android.Graphics.Color.Black);

            builder.SetPositiveButton(GetString(Android.Resource.String.Ok), (s, e) => { _noticeDialogListener.OnDialogPositiveClick(this); });
            builder.SetNegativeButton(GetString(Android.Resource.String.Cancel), (s, e) => { _noticeDialogListener.OnDialogNegativeClick(this); });

            AlertDialog dialog = builder.Create();
            dialog.Show();
            dialog.Window.SetSoftInputMode(SoftInput.StateAlwaysVisible);
            return dialog;
        }

        #endregion
    }
}