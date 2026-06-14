using AutoPartsStore.Api.Contracts.Companies;
using AutoPartsStore.Api.Contracts.Contacts;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class CompanyService(AppDbContext dbContext)
{
    public async Task<CompanyResponse> GetAsync(CancellationToken ct = default)
    {
        var company = await dbContext.Companies
            .AsNoTracking()
            .Include(c => c.Contacts)
            .FirstAsync(c => c.Id == Company.SingletonId, ct);

        return ToResponse(company);
    }

    public async Task<CompanyResponse> UpdateAsync(UpdateCompanyRequest request, CancellationToken ct = default)
    {
        var company = await dbContext.Companies
            .Include(c => c.Contacts)
            .FirstAsync(c => c.Id == Company.SingletonId, ct);

        company.Name = request.Name.Trim();
        company.Description = request.Description;
        company.Address = request.Address;

        await dbContext.SaveChangesAsync(ct);

        return ToResponse(company);
    }

    private static CompanyResponse ToResponse(Company company) =>
        new(
            company.Name,
            company.Description,
            company.Address,
            company.Contacts
                .OrderBy(c => c.Name)
                .Select(c => new ContactResponse(c.Id, c.Name, c.Description))
                .ToList());
}
