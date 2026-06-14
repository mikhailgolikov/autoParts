using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Attribute;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/attributes")]
public class AttributesController(AttributeService attributeService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<AttributeResponse>>> GetAll(CancellationToken ct) =>
        Ok(await attributeService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AttributeResponse>> GetById(Guid id, CancellationToken ct)
    {
        var attribute = await attributeService.GetByIdAsync(id, ct);
        return attribute is null ? NotFound() : Ok(attribute);
    }

    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<AttributeResponse>>> GetByCategoryId(
        Guid categoryId,
        CancellationToken ct)
    {
        try
        {
            return Ok(await attributeService.GetByCategoryIdAsync(categoryId, ct));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<AttributeResponse>> Create(
        [FromBody] CreateAttributeRequest request,
        CancellationToken ct)
    {
        try
        {
            var attribute = await attributeService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = attribute.Id }, attribute);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<AttributeResponse>> Update(
        Guid id,
        [FromBody] UpdateAttributeRequest request,
        CancellationToken ct)
    {
        try
        {
            var attribute = await attributeService.UpdateAsync(id, request, ct);
            return attribute is null ? NotFound() : Ok(attribute);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await attributeService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
