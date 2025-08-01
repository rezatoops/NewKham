namespace Datalayer.Entities
{
    public class ProductAttributeValue
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Value { get; set; }

        public ProductAttribute Attribute { get; set; }
    }
}
