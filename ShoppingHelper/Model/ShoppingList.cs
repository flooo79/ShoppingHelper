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
            Products = new List<ShoppingListProduct>();
        }

        #endregion

        #region Public Properties

        [Column("LastUpdated")]
        [NotNull]
        [Default]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ShoppingListProduct> Products { get; set; }

        #endregion
    }
}