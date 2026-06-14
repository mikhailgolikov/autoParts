namespace AutoPartsStore.Domain.Entities;

public class ProductCategory
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<ProductAttribute> Attributes { get; set; } = [];

    public ICollection<Product> Products { get; set; } = [];
}
