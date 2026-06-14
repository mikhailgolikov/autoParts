using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Contacts;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/company/contacts")]
public class ContactsController(ContactService contactService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ContactResponse>>> GetAll(CancellationToken ct) =>
        Ok(await contactService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ContactResponse>> GetById(Guid id, CancellationToken ct)
    {
        var contact = await contactService.GetByIdAsync(id, ct);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ContactResponse>> Create(
        [FromBody] CreateContactRequest request,
        CancellationToken ct)
    {
        var contact = await contactService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<ContactResponse>> Update(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken ct)
    {
        var contact = await contactService.UpdateAsync(id, request, ct);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await contactService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
