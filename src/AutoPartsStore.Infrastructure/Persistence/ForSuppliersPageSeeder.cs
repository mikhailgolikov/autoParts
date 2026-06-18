using Microsoft.EntityFrameworkCore;
using AutoPartsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPartsStore.Infrastructure.Persistence;

public static class ForSuppliersPageSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        if (await context.ForSuppliersPages.AnyAsync(ct))
            return;

        context.ForSuppliersPages.Add(new ForSuppliersPage
        {
            Id = ForSuppliersPage.SingletonId,
            Title = "Поставщикам",
            Content = "Текст страницы для поставщиков по умолчанию"
        });
        await context.SaveChangesAsync(ct);
    }
}