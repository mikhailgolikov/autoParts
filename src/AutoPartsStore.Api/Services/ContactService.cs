using AutoPartsStore.Api.Contracts.Contacts;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class ContactService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<ContactResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Contacts
            .AsNoTracking()
            .Where(c => c.CompanyId == Company.SingletonId)
            .OrderBy(c => c.Name)
            .Select(c => ToResponse(c))
            .ToListAsync(ct);

    public async Task<ContactResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await dbContext.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == Company.SingletonId, ct);

        return contact is null ? null : ToResponse(contact);
    }

    public async Task<ContactResponse> CreateAsync(CreateContactRequest request, CancellationToken ct = default)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            CompanyId = Company.SingletonId,
            Name = request.Name.Trim(),
            Description = request.Description
        };

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(ct);

        return ToResponse(contact);
    }

    public async Task<ContactResponse?> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken ct = default)
    {
        var contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == Company.SingletonId, ct);

        if (contact is null)
        {
            return null;
        }

        contact.Name = request.Name.Trim();
        contact.Description = request.Description;

        await dbContext.SaveChangesAsync(ct);

        return ToResponse(contact);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == Company.SingletonId, ct);

        if (contact is null)
        {
            return false;
        }

        dbContext.Contacts.Remove(contact);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static ContactResponse ToResponse(Contact contact) =>
        new(contact.Id, contact.Name, contact.Description);
}
