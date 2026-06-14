using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Product;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ProductService productService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductListItemResponse>>> GetAll(
        [FromQuery] ProductQuery query,
        CancellationToken ct) =>
        Ok(await productService.GetPagedAsync(query, ct));

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductListItemResponse>>> Search(
        [FromQuery] string q,
        [FromQuery] ProductQuery query,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search query 'q' is required." });
        }

        return Ok(await productService.SearchAsync(q, query, ct));
    }

    [HttpGet("brand/{brandId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductListItemResponse>>> GetByBrand(
        Guid brandId,
        [FromQuery] ProductQuery query,
        CancellationToken ct)
    {
        try
        {
            return Ok(await productService.GetByBrandIdAsync(brandId, query, ct));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductListItemResponse>>> GetByCategory(
        Guid categoryId,
        [FromQuery] ProductQuery query,
        CancellationToken ct)
    {
        try
        {
            return Ok(await productService.GetByCategoryIdAsync(categoryId, query, ct));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken ct)
    {
        var product = await productService.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        try
        {
            var product = await productService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ProductResponse>> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        try
        {
            var product = await productService.UpdateAsync(id, request, ct);
            return product is null ? NotFound() : Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ProductResponse>> UpdateStock(
        Guid id,
        [FromBody] UpdateProductStockRequest request,
        CancellationToken ct)
    {
        var product = await productService.UpdateStockAsync(id, request, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPatch("{id:guid}/price")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ProductResponse>> UpdatePrice(
        Guid id,
        [FromBody] UpdateProductPriceRequest request,
        CancellationToken ct)
    {
        var product = await productService.UpdatePriceAsync(id, request, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await productService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
