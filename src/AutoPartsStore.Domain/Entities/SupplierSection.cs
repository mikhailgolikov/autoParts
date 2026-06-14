namespace AutoPartsStore.Domain.Entities;

public class SupplierSection
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
