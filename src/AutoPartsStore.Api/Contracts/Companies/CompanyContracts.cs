using AutoPartsStore.Api.Contracts.Contacts;

namespace AutoPartsStore.Api.Contracts.Companies;

public record CompanyResponse(
    string Name,
    string? Description,
    string? Address,
    IReadOnlyList<ContactResponse> Contacts);

public record UpdateCompanyRequest(
    string Name,
    string? Description,
    string? Address);
