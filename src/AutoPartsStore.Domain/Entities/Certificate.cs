namespace AutoPartsStore.Domain.Entities;

/// <summary>
/// Сертификат — отдельная сущность без связей с другими таблицами.
/// </summary>
public class Certificate
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    /// <summary>Относительный путь к изображению сертификата.</summary>
    public string? ImagePath { get; set; }
}
