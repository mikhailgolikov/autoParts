using AutoPartsStore.Api.Contracts.Attribute;
using AutoPartsStore.Api.Contracts.Category;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class CategoryService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.ProductCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, null))
            .ToListAsync(ct);

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await dbContext.ProductCategories
            .AsNoTracking()
            .Include(c => c.Attributes)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return category is null ? null : ToResponse(category, includeAttributes: true);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        if (await dbContext.ProductCategories.AnyAsync(c => c.Name == name, ct))
        {
            throw new InvalidOperationException("Category with this name already exists.");
        }

        var entity = new ProductCategory { Id = Guid.NewGuid(), Name = name };
        dbContext.ProductCategories.Add(entity);
        await dbContext.SaveChangesAsync(ct);
        return new CategoryResponse(entity.Id, entity.Name);
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var entity = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return null;

        var name = request.Name.Trim();
        if (await dbContext.ProductCategories.AnyAsync(c => c.Name == name && c.Id != id, ct))
        {
            throw new InvalidOperationException("Category with this name already exists.");
        }

        entity.Name = name;
        await dbContext.SaveChangesAsync(ct);
        return new CategoryResponse(entity.Id, entity.Name);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;

        if (await dbContext.Products.AnyAsync(p => p.CategoryId == id, ct))
        {
            throw new InvalidOperationException("Cannot delete category with existing products.");
        }

        dbContext.ProductCategories.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    private static CategoryResponse ToResponse(ProductCategory category, bool includeAttributes) =>
        new(
            category.Id,
            category.Name,
            includeAttributes
                ? category.Attributes
                    .OrderBy(a => a.Name)
                    .Select(a => new AttributeResponse(a.Id, a.Name, a.Type, a.Unit, a.CategoryId))
                    .ToList()
                : null);
}
