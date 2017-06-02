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
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace ShoppingHelper.Model
{

    [Table("Product")]
    public class Product : IdDescriptionModel
    {
        public Product()
        {
            ShoppingLists = new List<ShoppingList>();
        }

        [ManyToMany(typeof(ShoppingListProduct))]
        public List<ShoppingList> ShoppingLists { get; set; }

        [Column("OrderId"), NotNull]
        public int OrderId { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; } = false;
    }
}