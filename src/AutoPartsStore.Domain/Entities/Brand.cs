namespace AutoPartsStore.Domain.Entities;

public class Brand
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
