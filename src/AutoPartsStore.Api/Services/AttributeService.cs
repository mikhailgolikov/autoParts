using AutoPartsStore.Api.Contracts.Attribute;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class AttributeService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<AttributeResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.ProductAttributes
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new AttributeResponse(a.Id, a.Name, a.Type, a.Unit, a.CategoryId))
            .ToListAsync(ct);

    public async Task<AttributeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var attribute = await dbContext.ProductAttributes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
        return attribute is null ? null : ToResponse(attribute);
    }

    public async Task<IReadOnlyList<AttributeResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        if (!await dbContext.ProductCategories.AnyAsync(c => c.Id == categoryId, ct))
        {
            throw new KeyNotFoundException("Category not found.");
        }

        return await dbContext.ProductAttributes
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId)
            .OrderBy(a => a.Name)
            .Select(a => new AttributeResponse(a.Id, a.Name, a.Type, a.Unit, a.CategoryId))
            .ToListAsync(ct);
    }

    public async Task<AttributeResponse> CreateAsync(CreateAttributeRequest request, CancellationToken ct = default)
    {
        if (!await dbContext.ProductCategories.AnyAsync(c => c.Id == request.CategoryId, ct))
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var name = request.Name.Trim();
        if (await dbContext.ProductAttributes.AnyAsync(a => a.CategoryId == request.CategoryId && a.Name == name, ct))
        {
            throw new InvalidOperationException("Attribute with this name already exists in the category.");
        }

        var entity = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Name = name,
            Type = request.Type,
            Unit = request.Unit?.Trim()
        };

        dbContext.ProductAttributes.Add(entity);
        await dbContext.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<AttributeResponse?> UpdateAsync(Guid id, UpdateAttributeRequest request, CancellationToken ct = default)
    {
        var entity = await dbContext.ProductAttributes.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (entity is null) return null;

        var name = request.Name.Trim();
        if (await dbContext.ProductAttributes.AnyAsync(
                a => a.CategoryId == entity.CategoryId && a.Name == name && a.Id != id, ct))
        {
            throw new InvalidOperationException("Attribute with this name already exists in the category.");
        }

        entity.Name = name;
        entity.Type = request.Type;
        entity.Unit = request.Unit?.Trim();
        await dbContext.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.ProductAttributes.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (entity is null) return false;

        dbContext.ProductAttributes.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    private static AttributeResponse ToResponse(ProductAttribute attribute) =>
        new(attribute.Id, attribute.Name, attribute.Type, attribute.Unit, attribute.CategoryId);
}
