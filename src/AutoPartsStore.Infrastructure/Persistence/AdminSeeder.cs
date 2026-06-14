using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AutoPartsStore.Infrastructure.Persistence;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        AppDbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin, cancellationToken))
        {
            return;
        }

        var email = configuration["Seed:Admin:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["Seed:Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (await dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            return;
        }

        var name = configuration["Seed:Admin:Name"]?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = email.Split('@')[0];
        }

        dbContext.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            PhoneNumber = configuration["Seed:Admin:PhoneNumber"]?.Trim(),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
