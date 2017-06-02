﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShoppingHelper
{
    public interface IItemTouchHelperAdapter
    {
        void OnItemMove(int fromPosition, int toPosition);

        void OnItemDismiss(int position);
    }
}