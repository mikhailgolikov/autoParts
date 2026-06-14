namespace AutoPartsStore.Api.Contracts.Brand;

public record BrandResponse(Guid Id, string Name);

public record CreateBrandRequest(string Name);

public record UpdateBrandRequest(string Name);
