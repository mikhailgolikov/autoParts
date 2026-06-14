using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public UserRole Role { get; set; } = UserRole.Client;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appeal> Appeals { get; set; } = [];
}
