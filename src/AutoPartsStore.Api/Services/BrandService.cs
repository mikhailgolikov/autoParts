using AutoPartsStore.Api.Contracts.Brand;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class BrandService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<BrandResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandResponse(b.Id, b.Name))
            .ToListAsync(ct);

    public async Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var brand = await dbContext.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
        return brand is null ? null : new BrandResponse(brand.Id, brand.Name);
    }

    public async Task<BrandResponse> CreateAsync(CreateBrandRequest request, CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        if (await dbContext.Brands.AnyAsync(b => b.Name == name, ct))
        {
            throw new InvalidOperationException("Brand with this name already exists.");
        }

        var entity = new Brand { Id = Guid.NewGuid(), Name = name };
        dbContext.Brands.Add(entity);
        await dbContext.SaveChangesAsync(ct);
        return new BrandResponse(entity.Id, entity.Name);
    }

    public async Task<BrandResponse?> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken ct = default)
    {
        var entity = await dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (entity is null) return null;

        var name = request.Name.Trim();
        if (await dbContext.Brands.AnyAsync(b => b.Name == name && b.Id != id, ct))
        {
            throw new InvalidOperationException("Brand with this name already exists.");
        }

        entity.Name = name;
        await dbContext.SaveChangesAsync(ct);
        return new BrandResponse(entity.Id, entity.Name);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (entity is null) return false;

        if (await dbContext.Products.AnyAsync(p => p.BrandId == id, ct))
        {
            throw new InvalidOperationException("Cannot delete brand with existing products.");
        }

        dbContext.Brands.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
