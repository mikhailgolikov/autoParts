using AutoPartsStore.Api.Contracts.Catalog;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class SiteContentService(AppDbContext dbContext)
{
    // Supplier sections
    public async Task<IReadOnlyList<SupplierSectionResponse>> GetSupplierSectionsAsync(CancellationToken ct = default) =>
        await dbContext.SupplierSections.AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SupplierSectionResponse(s.Id, s.Name, s.Description))
            .ToListAsync(ct);

    public async Task<SupplierSectionResponse> CreateSupplierSectionAsync(CreateSupplierSectionRequest req, CancellationToken ct = default)
    {
        var entity = new SupplierSection { Id = Guid.NewGuid(), Name = req.Name.Trim(), Description = req.Description };
        dbContext.SupplierSections.Add(entity);
        await dbContext.SaveChangesAsync(ct);
        return new SupplierSectionResponse(entity.Id, entity.Name, entity.Description);
    }

    public async Task<SupplierSectionResponse?> UpdateSupplierSectionAsync(Guid id, UpdateSupplierSectionRequest req, CancellationToken ct = default)
    {
        var entity = await dbContext.SupplierSections.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;
        entity.Name = req.Name.Trim();
        entity.Description = req.Description;
        await dbContext.SaveChangesAsync(ct);
        return new SupplierSectionResponse(entity.Id, entity.Name, entity.Description);
    }

    public async Task<bool> DeleteSupplierSectionAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.SupplierSections.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;
        dbContext.SupplierSections.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

}
