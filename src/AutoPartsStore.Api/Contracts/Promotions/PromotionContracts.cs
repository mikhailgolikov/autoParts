namespace AutoPartsStore.Api.Contracts.Promotions;

public record PromotionResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime PublishedAt,
    string? ImagePath);

public record CreatePromotionRequest(
    string Name,
    string? Description,
    DateTime? PublishedAt,
    string? ImagePath);

public record UpdatePromotionRequest(
    string Name,
    string? Description,
    DateTime? PublishedAt,
    string? ImagePath);
