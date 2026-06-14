using AutoPartsStore.Api.Contracts.Certificates;
using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Api.Services;

public class CertificateService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<CertificateResponse>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Certificates
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => ToResponse(c))
            .ToListAsync(ct);

    public async Task<CertificateResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var certificate = await dbContext.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return certificate is null ? null : ToResponse(certificate);
    }

    public async Task<CertificateResponse> CreateAsync(CreateCertificateRequest request, CancellationToken ct = default)
    {
        var entity = new Certificate
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ImagePath = request.ImagePath
        };

        dbContext.Certificates.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<CertificateResponse?> UpdateAsync(
        Guid id,
        UpdateCertificateRequest request,
        CancellationToken ct = default)
    {
        var entity = await dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.ImagePath = request.ImagePath;

        await dbContext.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
        {
            return false;
        }

        dbContext.Certificates.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static CertificateResponse ToResponse(Certificate certificate) =>
        new(certificate.Id, certificate.Name, certificate.ImagePath);
}
