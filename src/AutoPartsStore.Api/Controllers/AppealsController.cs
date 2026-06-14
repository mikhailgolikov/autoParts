using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Appeals;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/appeals")]
public class AppealsController(AppealService appealService, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await appealService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<object>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var appeal = await appealService.GetByIdAsync(id, cancellationToken);
            return appeal is null ? NotFound() : Ok(appeal);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("my")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetMyAppeals(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        return Ok(await appealService.GetByUserIdAsync(currentUser.UserId.Value, cancellationToken));
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken) =>
        Ok(await appealService.GetByUserIdAsync(userId, cancellationToken));

    [HttpPost("client-questions")]
    public async Task<ActionResult<ClientQuestionResponse>> CreateClientQuestion(
        [FromBody] CreateClientQuestionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appeal = await appealService.CreateClientQuestionAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = appeal.Id }, appeal);
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

    [HttpPost("supplier-requests")]
    public async Task<ActionResult<SupplierRequestResponse>> CreateSupplierRequest(
        [FromBody] CreateSupplierRequestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appeal = await appealService.CreateSupplierRequestAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = appeal.Id }, appeal);
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

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<object>> UpdateStatus(
        Guid id,
        [FromBody] UpdateAppealStatusRequest request,
        CancellationToken cancellationToken)
    {
        var appeal = await appealService.UpdateStatusAsync(id, request, cancellationToken);
        return appeal is null ? NotFound() : Ok(appeal);
    }

    [HttpPatch("{id:guid}/contacts")]
    public async Task<ActionResult<object>> UpdateContacts(
        Guid id,
        [FromBody] UpdateAppealContactsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var appeal = await appealService.UpdateContactsAsync(id, request, cancellationToken);
            return appeal is null ? NotFound() : Ok(appeal);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
