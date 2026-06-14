using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Domain.Entities;

public abstract class Appeal
{
    public Guid Id { get; set; }

    public required string Number { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public required string ContactPhone { get; set; }

    public required string ContactEmail { get; set; }

    public required string ManagerComment { get; set; }

    public AppealStatus Status { get; set; } = AppealStatus.New;
}
