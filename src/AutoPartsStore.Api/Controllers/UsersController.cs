using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Users;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController(UserService userService, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var user = await userService.GetByIdAsync(currentUser.UserId.Value, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("me/profile")]
    public async Task<ActionResult<UserResponse>> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        try
        {
            var user = await userService.UpdateProfileAsync(currentUser.UserId.Value, request, cancellationToken);
            return user is null ? NotFound() : Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        try
        {
            await userService.ChangePasswordAsync(currentUser.UserId.Value, request, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await userService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAdmin && currentUser.UserId != id)
        {
            return Forbid();
        }

        var user = await userService.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.CreateByAdminAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAdmin && currentUser.UserId != id)
        {
            return Forbid();
        }

        var user = await userService.UpdateAsync(id, request, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<UserResponse>> ChangeRole(
        Guid id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.ChangeRoleAsync(id, request, cancellationToken);
            return user is null ? NotFound() : Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        try
        {
            return await userService.DeleteAsync(id, currentUser.UserId.Value, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
