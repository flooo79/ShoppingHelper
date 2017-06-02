namespace ShoppingHelper.Model
{
    using SQLite.Net.Attributes;

    using SQLiteNetExtensions.Attributes;

    [Table("ShoppingListProduct")]
    public class ShoppingListProduct
    {
        #region Public Properties

        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }

        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(typeof(ShoppingList))]
        public int ShoppingListId { get; set; }

        #endregion
    }
}