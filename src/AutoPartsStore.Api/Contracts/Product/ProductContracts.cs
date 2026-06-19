using AutoPartsStore.Domain.Enums;

namespace AutoPartsStore.Api.Contracts.Product;

public record ProductAttributeValueResponse(
    Guid AttributeId,
    string AttributeName,
    AttributeType Type,
    string? Unit,
    string Value);

public record ProductBrandResponse(Guid Id, string Name);

public record ProductCategoryResponse(Guid Id, string Name);

public record ProductListItemResponse(
    Guid Id,
    string Name,
    string Article,
    int Quantity,
    decimal Price,
    bool InStock,
    string? ImagePath,
    DateTime CreatedAt,
    ProductBrandResponse Brand,
    ProductCategoryResponse Category);

public record ProductResponse(
    Guid Id,
    string Name,
    string Article,
    int Quantity,
    decimal Price,
    bool InStock,
    string? Description,
    string? ImagePath,
    DateTime CreatedAt,
    ProductBrandResponse Brand,
    ProductCategoryResponse Category,
    IReadOnlyList<ProductAttributeValueResponse> AttributeValues);

public record ProductAttributeValueInput(Guid AttributeId, string Value);

public record CreateProductRequest(
    string Name,
    string Article,
    int Quantity,
    decimal Price,
    bool InStock,
    string? Description,
    string? ImagePath,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<ProductAttributeValueInput>? AttributeValues);

public record UpdateProductRequest(
    string Name,
    string Article,
    int Quantity,
    decimal Price,
    bool InStock,
    string? Description,
    string? ImagePath,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<ProductAttributeValueInput>? AttributeValues);

public record UpdateProductStockRequest(int Quantity, bool InStock);

public record UpdateProductPriceRequest(decimal Price);

public record AttributeFilterQuery(
    Guid AttributeId,
    string? Min = null,
    string? Max = null,
    string? Value = null);

public record ProductQuery(
    Guid? BrandId = null,
    Guid? CategoryId = null,
    string? Name = null,
    bool? InStock = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "createdAt",
    string SortOrder = "desc",
    IReadOnlyList<AttributeFilterQuery>? AttributeFilters = null);

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
