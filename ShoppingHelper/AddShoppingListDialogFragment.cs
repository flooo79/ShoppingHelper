using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace ShoppingHelper
{
    public class AddShoppingListDialogFragment : DialogFragment
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            View view = Activity.LayoutInflater.Inflate(Resource.Layout.AddShoppingList, null);
            
            builder.SetTitle(GetString(Resource.String.ShoppingListName));
            builder.SetView(view);

            EditText editText = view.FindViewById<EditText>(Resource.Id.AddShoppingListNameEditText);
                
            editText.RequestFocus();
            editText.SetTextColor(Android.Graphics.Color.Black);
            editText.Text = GetString(Resource.String.NewShoppingList);
            editText.SelectAll();

            builder.SetPositiveButton(GetString(Android.Resource.String.Ok), (s, e) => { _noticeDialogListener.OnDialogPositiveClick(this); });
            builder.SetNegativeButton(GetString(Android.Resource.String.Cancel), (s, e) => { _noticeDialogListener.OnDialogNegativeClick(this); });

            AlertDialog dialog = builder.Create();
            return dialog;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);

            try
            {
                _noticeDialogListener = (INoticeDialogListener)activity;
            }
            catch (ClassCastException e)
            {
                throw new ClassCastException(activity.ToString() + " must implement INoticeDialogListener");
            }
        }

        private INoticeDialogListener _noticeDialogListener;
    }
}