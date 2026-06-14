namespace AutoPartsStore.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Article { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool InStock { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid BrandId { get; set; }

    public Brand Brand { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = [];
}
