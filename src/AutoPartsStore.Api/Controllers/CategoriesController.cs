using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Category;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(CategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(CancellationToken ct) =>
        Ok(await categoryService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(id, ct);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<CategoryResponse>> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken ct)
    {
        try
        {
            var category = await categoryService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<CategoryResponse>> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken ct)
    {
        try
        {
            var category = await categoryService.UpdateAsync(id, request, ct);
            return category is null ? NotFound() : Ok(category);
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
            return await categoryService.DeleteAsync(id, ct) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
