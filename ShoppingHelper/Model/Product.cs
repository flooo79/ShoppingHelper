namespace ShoppingHelper.Model
{
    using System.Collections.Generic;

    using SQLite.Net.Attributes;

    using SQLiteNetExtensions.Attributes;

    [Table("Product")]
    public class Product : IdDescriptionModel
    {
        #region Constructors and Destructors

        public Product()
        {
            ShoppingLists = new List<ShoppingList>();
        }

        #endregion

        #region Public Properties

        [Column("IsSelected")]
        public bool IsSelected { get; set; } = false;

        [Column("OrderId"), NotNull]
        public int OrderId { get; set; }

        [ManyToMany(typeof(ShoppingListProduct))]
        public List<ShoppingList> ShoppingLists { get; set; }

        #endregion
    }
}