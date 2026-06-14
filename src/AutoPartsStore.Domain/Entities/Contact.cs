namespace AutoPartsStore.Domain.Entities;

/// <summary>
/// Контакт компании (телефон, Telegram, VK и т.д.).
/// Связь только через CompanyId — без обратной навигации, чтобы не было циклов.
/// </summary>
public class Contact
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public Guid CompanyId { get; set; }
}
