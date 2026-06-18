namespace AutoPartsStore.Api.Contracts.ForSuppliersPage;

public record ForSuppliersPageResponse(
    string Title,
    string Content);

public record UpdateForSuppliersPageRequest(
    string Title,
    string Content);