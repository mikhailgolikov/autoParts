namespace AutoPartsStore.Api.Contracts.Contacts;

public record ContactResponse(
    Guid Id,
    string Name,
    string? Description);

public record CreateContactRequest(
    string Name,
    string? Description);

public record UpdateContactRequest(
    string Name,
    string? Description);
