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

using SQLite;
using ShoppingHelper.Model;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace ShoppingHelper
{

    [Table("ShoppingList")]
    public class ShoppingList : IdDescriptionModel
    {
        public ShoppingList()
        {
            Products = new List<Product>();
        }

        [ManyToMany(typeof(ShoppingListProduct), CascadeOperations = CascadeOperation.All)]
        public List<Product> Products { get; set; }

        [Column("Count")]
        public int Count { get; set; } = 0;

        [Column("LastUpdated")]
        [NotNull]
        [Default(usePropertyValue: true)]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}