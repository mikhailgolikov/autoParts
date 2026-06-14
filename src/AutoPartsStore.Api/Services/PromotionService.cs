using AutoPartsStore.Api.Contracts.Promotions;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class PromotionService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<PromotionResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Promotions
            .AsNoTracking()
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => ToResponse(p))
            .ToListAsync(ct);

    public async Task<PromotionResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var promotion = await dbContext.Promotions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        return promotion is null ? null : ToResponse(promotion);
    }

    public async Task<PromotionResponse> CreateAsync(CreatePromotionRequest request, CancellationToken ct = default)
    {
        var entity = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description,
            PublishedAt = request.PublishedAt ?? DateTime.UtcNow,
            ImagePath = request.ImagePath
        };

        dbContext.Promotions.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<PromotionResponse?> UpdateAsync(Guid id, UpdatePromotionRequest request, CancellationToken ct = default)
    {
        var entity = await dbContext.Promotions.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description;
        if (request.PublishedAt.HasValue)
        {
            entity.PublishedAt = request.PublishedAt.Value;
        }

        entity.ImagePath = request.ImagePath;

        await dbContext.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.Promotions.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null)
        {
            return false;
        }

        dbContext.Promotions.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static PromotionResponse ToResponse(Promotion promotion) =>
        new(promotion.Id, promotion.Name, promotion.Description, promotion.PublishedAt, promotion.ImagePath);
}
