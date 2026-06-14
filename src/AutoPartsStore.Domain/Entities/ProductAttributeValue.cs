namespace AutoPartsStore.Domain.Entities;

public class ProductAttributeValue
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public Guid AttributeId { get; set; }

    public ProductAttribute Attribute { get; set; } = null!;

    public required string Value { get; set; }
}
