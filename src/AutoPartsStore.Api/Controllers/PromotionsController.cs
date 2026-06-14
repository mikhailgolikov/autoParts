using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Promotions;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(PromotionService promotionService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PromotionResponse>>> GetAll(CancellationToken ct) =>
        Ok(await promotionService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PromotionResponse>> GetById(Guid id, CancellationToken ct)
    {
        var promotion = await promotionService.GetByIdAsync(id, ct);
        return promotion is null ? NotFound() : Ok(promotion);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<PromotionResponse>> Create(
        [FromBody] CreatePromotionRequest request,
        CancellationToken ct)
    {
        var promotion = await promotionService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<PromotionResponse>> Update(
        Guid id,
        [FromBody] UpdatePromotionRequest request,
        CancellationToken ct)
    {
        var promotion = await promotionService.UpdateAsync(id, request, ct);
        return promotion is null ? NotFound() : Ok(promotion);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await promotionService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
