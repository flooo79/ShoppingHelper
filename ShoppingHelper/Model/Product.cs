namespace ShoppingHelper.Model
{
    using SQLite.Net.Attributes;

    using SQLiteNetExtensions.Attributes;

    [Table("Product")]
    public class Product : IdDescriptionModel
    {
        #region Public Properties

        [Column("OrderId"), NotNull]
        public int OrderId { get; set; }

        #endregion
    }
}