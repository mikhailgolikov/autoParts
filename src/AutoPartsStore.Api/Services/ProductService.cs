using AutoPartsStore.Api.Contracts.Product;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Helpers;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class ProductService(AppDbContext dbContext)
{
    public async Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(
        ProductQuery query,
        CancellationToken ct = default)
    {
        var productsQuery = BuildBaseQuery(query.BrandId, query.CategoryId);
        productsQuery = ApplySorting(productsQuery, query.SortBy, query.SortOrder);

        var products = await productsQuery
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.AttributeValues)
            .ThenInclude(v => v.Attribute)
            .AsNoTracking()
            .ToListAsync(ct);

        if (query.AttributeFilters?.Count > 0)
        {
            products = ApplyAttributeFilters(products, query.AttributeFilters);
        }

        var totalCount = products.Count;
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItemResponse)
            .ToList();

        return new PagedResponse<ProductListItemResponse>(items, totalCount, page, pageSize);
    }

    public async Task<PagedResponse<ProductListItemResponse>> SearchAsync(
        string searchTerm,
        ProductQuery query,
        CancellationToken ct = default)
    {
        var term = searchTerm.Trim().ToLowerInvariant();
        var productsQuery = BuildBaseQuery(query.BrandId, query.CategoryId)
            .Where(p => p.Name.ToLower().Contains(term) || p.Article.ToLower().Contains(term));

        productsQuery = ApplySorting(productsQuery, query.SortBy, query.SortOrder);

        var products = await productsQuery
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.AttributeValues)
            .ThenInclude(v => v.Attribute)
            .AsNoTracking()
            .ToListAsync(ct);

        if (query.AttributeFilters?.Count > 0)
        {
            products = ApplyAttributeFilters(products, query.AttributeFilters);
        }

        var totalCount = products.Count;
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItemResponse)
            .ToList();

        return new PagedResponse<ProductListItemResponse>(items, totalCount, page, pageSize);
    }

    public async Task<PagedResponse<ProductListItemResponse>> GetByBrandIdAsync(
        Guid brandId,
        ProductQuery query,
        CancellationToken ct = default)
    {
        if (!await dbContext.Brands.AnyAsync(b => b.Id == brandId, ct))
        {
            throw new KeyNotFoundException("Brand not found.");
        }

        return await GetPagedAsync(query with { BrandId = brandId }, ct);
    }

    public async Task<PagedResponse<ProductListItemResponse>> GetByCategoryIdAsync(
        Guid categoryId,
        ProductQuery query,
        CancellationToken ct = default)
    {
        if (!await dbContext.ProductCategories.AnyAsync(c => c.Id == categoryId, ct))
        {
            throw new KeyNotFoundException("Category not found.");
        }

        return await GetPagedAsync(query with { CategoryId = categoryId }, ct);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.AttributeValues)
            .ThenInclude(v => v.Attribute)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return product is null ? null : ToDetailResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        await EnsureBrandAndCategoryExistAsync(request.BrandId, request.CategoryId, ct);

        if (await dbContext.Products.AnyAsync(p => p.Article == request.Article.Trim(), ct))
        {
            throw new InvalidOperationException("Product with this article already exists.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Article = request.Article.Trim(),
            Quantity = request.Quantity,
            Price = request.Price,
            InStock = request.InStock,
            Description = request.Description,
            ImagePath = request.ImagePath,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);

        if (request.AttributeValues?.Count > 0)
        {
            await SetAttributeValuesAsync(product.Id, product.CategoryId, request.AttributeValues, ct);
        }

        return (await GetByIdAsync(product.Id, ct))!;
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return null;

        await EnsureBrandAndCategoryExistAsync(request.BrandId, request.CategoryId, ct);

        var article = request.Article.Trim();
        if (await dbContext.Products.AnyAsync(p => p.Article == article && p.Id != id, ct))
        {
            throw new InvalidOperationException("Product with this article already exists.");
        }

        product.Name = request.Name.Trim();
        product.Article = article;
        product.Quantity = request.Quantity;
        product.Price = request.Price;
        product.InStock = request.InStock;
        product.Description = request.Description;
        product.ImagePath = request.ImagePath;
        product.BrandId = request.BrandId;
        product.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync(ct);

        if (request.AttributeValues is not null)
        {
            var existing = await dbContext.ProductAttributeValues
                .Where(v => v.ProductId == id)
                .ToListAsync(ct);
            dbContext.ProductAttributeValues.RemoveRange(existing);
            await dbContext.SaveChangesAsync(ct);

            if (request.AttributeValues.Count > 0)
            {
                await SetAttributeValuesAsync(id, request.CategoryId, request.AttributeValues, ct);
            }
        }

        return await GetByIdAsync(id, ct);
    }

    public async Task<ProductResponse?> UpdateStockAsync(
        Guid id,
        UpdateProductStockRequest request,
        CancellationToken ct = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return null;

        product.Quantity = request.Quantity;
        product.InStock = request.InStock;
        await dbContext.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<ProductResponse?> UpdatePriceAsync(
        Guid id,
        UpdateProductPriceRequest request,
        CancellationToken ct = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return null;

        product.Price = request.Price;
        await dbContext.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return false;

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    private IQueryable<Product> BuildBaseQuery(Guid? brandId, Guid? categoryId)
    {
        var query = dbContext.Products.AsQueryable();

        if (brandId.HasValue)
        {
            query = query.Where(p => p.BrandId == brandId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return query;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, string sortOrder)
    {
        var descending = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            _ => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };
    }

    private static List<Product> ApplyAttributeFilters(
        List<Product> products,
        IReadOnlyList<AttributeFilterQuery> filters)
    {
        foreach (var filter in filters)
        {
            products = products.Where(p =>
                p.AttributeValues.Any(v =>
                    v.AttributeId == filter.AttributeId &&
                    AttributeValueParser.MatchesFilter(
                        v.Value,
                        v.Attribute.Type,
                        filter.Min,
                        filter.Max,
                        filter.Value)))
                .ToList();
        }

        return products;
    }

    private async Task SetAttributeValuesAsync(
        Guid productId,
        Guid categoryId,
        IReadOnlyList<ProductAttributeValueInput> values,
        CancellationToken ct)
    {
        var attributes = await dbContext.ProductAttributes
            .Where(a => a.CategoryId == categoryId)
            .ToDictionaryAsync(a => a.Id, ct);

        foreach (var item in values)
        {
            if (!attributes.TryGetValue(item.AttributeId, out var attribute))
            {
                throw new InvalidOperationException(
                    $"Attribute {item.AttributeId} does not belong to product category.");
            }

            if (!AttributeValueParser.IsValidForType(item.Value, attribute.Type))
            {
                throw new InvalidOperationException(
                    $"Value '{item.Value}' is invalid for attribute '{attribute.Name}' of type {attribute.Type}.");
            }

            var normalizedValue = AttributeValueParser.NormalizeToString(item.Value, attribute.Type);

            dbContext.ProductAttributeValues.Add(new ProductAttributeValue
            {
                ProductId = productId,
                AttributeId = item.AttributeId,
                Value = normalizedValue
            });
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task EnsureBrandAndCategoryExistAsync(Guid brandId, Guid categoryId, CancellationToken ct)
    {
        if (!await dbContext.Brands.AnyAsync(b => b.Id == brandId, ct))
        {
            throw new KeyNotFoundException("Brand not found.");
        }

        if (!await dbContext.ProductCategories.AnyAsync(c => c.Id == categoryId, ct))
        {
            throw new KeyNotFoundException("Category not found.");
        }
    }

    private static ProductListItemResponse ToListItemResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Article,
            product.Quantity,
            product.Price,
            product.InStock,
            product.ImagePath,
            product.CreatedAt,
            new ProductBrandResponse(product.Brand.Id, product.Brand.Name),
            new ProductCategoryResponse(product.Category.Id, product.Category.Name));

    private static ProductResponse ToDetailResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Article,
            product.Quantity,
            product.Price,
            product.InStock,
            product.Description,
            product.ImagePath,
            product.CreatedAt,
            new ProductBrandResponse(product.Brand.Id, product.Brand.Name),
            new ProductCategoryResponse(product.Category.Id, product.Category.Name),
            product.AttributeValues
                .OrderBy(v => v.Attribute.Name)
                .Select(v => new ProductAttributeValueResponse(
                    v.AttributeId,
                    v.Attribute.Name,
                    v.Attribute.Type,
                    v.Attribute.Unit,
                    v.Value))
                .ToList());
}
