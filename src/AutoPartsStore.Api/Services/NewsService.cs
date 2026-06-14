using AutoPartsStore.Api.Contracts.News;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class NewsService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<NewsResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.News
            .AsNoTracking()
            .OrderByDescending(n => n.PublishedAt)
            .Select(n => ToResponse(n))
            .ToListAsync(ct);

    public async Task<NewsResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var news = await dbContext.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id, ct);
        return news is null ? null : ToResponse(news);
    }

    public async Task<NewsResponse> CreateAsync(CreateNewsRequest request, CancellationToken ct = default)
    {
        var entity = new News
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description,
            PublishedAt = request.PublishedAt ?? DateTime.UtcNow,
            ImagePath = request.ImagePath
        };

        dbContext.News.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<NewsResponse?> UpdateAsync(Guid id, UpdateNewsRequest request, CancellationToken ct = default)
    {
        var entity = await dbContext.News.FirstOrDefaultAsync(n => n.Id == id, ct);
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
        var entity = await dbContext.News.FirstOrDefaultAsync(n => n.Id == id, ct);
        if (entity is null)
        {
            return false;
        }

        dbContext.News.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static NewsResponse ToResponse(News news) =>
        new(news.Id, news.Name, news.Description, news.PublishedAt, news.ImagePath);
}
