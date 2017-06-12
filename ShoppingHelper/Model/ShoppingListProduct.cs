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

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Product Product { get; set; }

        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }

        [Column("Quantity")]
        [NotNull]
        [Default(value: 0)]
        public int Quantity { get; set; }

        [ForeignKey(typeof(ShoppingList))]
        public int ShoppingListId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return Product != null ? $"{Product.Description ?? string.Empty} ({Quantity:0})" : base.ToString();
        }

        #endregion
    }
}