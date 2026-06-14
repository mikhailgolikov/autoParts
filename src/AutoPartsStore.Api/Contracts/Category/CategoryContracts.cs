using AutoPartsStore.Api.Contracts.Attribute;

namespace AutoPartsStore.Api.Contracts.Category;

public record CategoryResponse(
    Guid Id,
    string Name,
    IReadOnlyList<AttributeResponse>? Attributes = null);

public record CreateCategoryRequest(string Name);

public record UpdateCategoryRequest(string Name);
