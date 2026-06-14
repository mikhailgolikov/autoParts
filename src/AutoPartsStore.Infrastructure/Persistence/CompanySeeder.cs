using AutoPartsStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Infrastructure.Persistence;

public static class CompanySeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Companies.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Companies.Add(new Company
        {
            Id = Company.SingletonId,
            Name = "Автозапчасти"
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
