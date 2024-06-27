namespace Market.Models.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ProductGroup? ProductGroup { get; set; }
    }
}
