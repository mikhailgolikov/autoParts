using AutoPartsStore.Api.Contracts.ForSuppliersPage;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class ForSuppliersPageService(AppDbContext ctx)
{
    public async Task<ForSuppliersPageResponse> GetAsync(CancellationToken ct = default)
    {
        var page = await ctx.ForSuppliersPages
            .AsNoTracking()
            .FirstAsync(p => p.Id == ForSuppliersPage.SingletonId, ct);

        return ToResponse(page);
    }

    public async Task<ForSuppliersPageResponse> UpdateAsync(
        UpdateForSuppliersPageRequest request,
        CancellationToken ct = default)
    {
        var page = await ctx.ForSuppliersPages
            .FirstAsync(p => p.Id == ForSuppliersPage.SingletonId, ct);

        page.Title = request.Title.Trim();
        page.Content = request.Content;

        await ctx.SaveChangesAsync(ct);

        return ToResponse(page);
    }

    private static ForSuppliersPageResponse ToResponse(ForSuppliersPage page) =>
        new(page.Title, page.Content);
}