namespace AutoPartsStore.Api.Contracts.News;

public record NewsResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime PublishedAt,
    string? ImagePath);

public record CreateNewsRequest(
    string Name,
    string? Description,
    DateTime? PublishedAt,
    string? ImagePath);

public record UpdateNewsRequest(
    string Name,
    string? Description,
    DateTime? PublishedAt,
    string? ImagePath);
