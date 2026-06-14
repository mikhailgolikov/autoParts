using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Domain.Entities;

public class ProductAttribute
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public AttributeType Type { get; set; } = AttributeType.String;

    public string? Unit { get; set; }

    public Guid CategoryId { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public ICollection<ProductAttributeValue> ProductValues { get; set; } = [];
}
