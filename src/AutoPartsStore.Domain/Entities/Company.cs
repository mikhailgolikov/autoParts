namespace AutoPartsStore.Domain.Entities;

/// <summary>
/// Единственная компания, для которой создан этот сайт.
/// </summary>
public class Company
{
    public static readonly Guid SingletonId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public ICollection<Contact> Contacts { get; set; } = [];
}
