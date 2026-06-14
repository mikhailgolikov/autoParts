namespace AutoPartsStore.Domain.Entities;

public abstract class Content
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Относительный путь к изображению.</summary>
    public string? ImagePath { get; set; }
}
