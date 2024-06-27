namespace Market.Models
{
    public class Product : BaseModel
    {
        public int Cost { get; set; }
        public int ProductGroupId { get; set; }
        public virtual ProductGroup? ProductGroup { get; set; }
        public virtual List<Storage> Storages { get; set; } = new List<Storage>();
    }
}
