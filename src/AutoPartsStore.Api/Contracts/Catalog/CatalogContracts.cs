namespace AutoPartsStore.Api.Contracts.Catalog;

public record SupplierSectionResponse(Guid Id, string Name, string? Description);
public record CreateSupplierSectionRequest(string Name, string? Description);
public record UpdateSupplierSectionRequest(string Name, string? Description);
