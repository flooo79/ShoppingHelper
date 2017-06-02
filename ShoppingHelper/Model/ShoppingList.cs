namespace ShoppingHelper.Model
{
    using System;
    using System.Collections.Generic;

    using SQLite.Net.Attributes;

    using SQLiteNetExtensions.Attributes;

    [Table("ShoppingList")]
    public class ShoppingList : IdDescriptionModel
    {
        #region Constructors and Destructors

        public ShoppingList()
        {
            Products = new List<Product>();
        }

        #endregion

        #region Public Properties

        [Column("Count")]
        public int Count { get; set; } = 0;

        [Column("LastUpdated")]
        [NotNull]
        [Default]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [ManyToMany(typeof(ShoppingListProduct), CascadeOperations = CascadeOperation.All)]
        public List<Product> Products { get; set; }

        #endregion
    }
}