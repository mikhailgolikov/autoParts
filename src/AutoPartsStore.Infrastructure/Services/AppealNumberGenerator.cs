using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Infrastructure.Services;

public class AppealNumberGenerator(AppDbContext dbContext)
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var prefix = $"APL-{today:yyyyMMdd}-";

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var counter = await dbContext.AppealNumberCounters
            .FirstOrDefaultAsync(c => c.Date == today, cancellationToken);

        if (counter is null)
        {
            counter = new AppealNumberCounter { Date = today, LastNumber = 1 };
            dbContext.AppealNumberCounters.Add(counter);
        }
        else
        {
            counter.LastNumber++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return $"{prefix}{counter.LastNumber:D4}";
    }
}
