namespace AutoPartsStore.Api.Contracts.Certificates;

public record CertificateResponse(
    Guid Id,
    string Name,
    string? ImagePath);

public record CreateCertificateRequest(
    string Name,
    string? ImagePath);

public record UpdateCertificateRequest(
    string Name,
    string? ImagePath);
