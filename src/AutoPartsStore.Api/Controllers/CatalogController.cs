using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Catalog;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(SiteContentService siteContent) : ControllerBase
{
    [HttpGet("supplier-sections")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<SupplierSectionResponse>>> GetSupplierSections(CancellationToken ct) =>
        Ok(await siteContent.GetSupplierSectionsAsync(ct));

    [HttpPost("supplier-sections")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<SupplierSectionResponse>> CreateSupplierSection(
        [FromBody] CreateSupplierSectionRequest request, CancellationToken ct) =>
        Ok(await siteContent.CreateSupplierSectionAsync(request, ct));
}
