namespace ShoppingHelper.Model
{
    using SQLite.Net.Attributes;

    public class IdDescriptionModel
    {
        #region Public Properties

        [Column("Description"), MaxLength(128), NotNull]
        public string Description { get; set; }

        [PrimaryKey, Unique, AutoIncrement, NotNull]
        public int Id { get; set; }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            IdDescriptionModel model = obj as IdDescriptionModel;

            if (model == null)
            {
                return false;
            }

            return GetType() == model.GetType() && Id == model.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id} - {Description}";
        }

        #endregion
    }
}