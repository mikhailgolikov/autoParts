using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Brand;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController(BrandService brandService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<BrandResponse>>> GetAll(CancellationToken ct) =>
        Ok(await brandService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BrandResponse>> GetById(Guid id, CancellationToken ct)
    {
        var brand = await brandService.GetByIdAsync(id, ct);
        return brand is null ? NotFound() : Ok(brand);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<BrandResponse>> Create(
        [FromBody] CreateBrandRequest request,
        CancellationToken ct)
    {
        try
        {
            var brand = await brandService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<BrandResponse>> Update(
        Guid id,
        [FromBody] UpdateBrandRequest request,
        CancellationToken ct)
    {
        try
        {
            var brand = await brandService.UpdateAsync(id, request, ct);
            return brand is null ? NotFound() : Ok(brand);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            return await brandService.DeleteAsync(id, ct) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
