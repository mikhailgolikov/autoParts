using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Certificates;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/certificates")]
public class CertificatesController(CertificateService certificateService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CertificateResponse>>> GetAll(CancellationToken ct) =>
        Ok(await certificateService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CertificateResponse>> GetById(Guid id, CancellationToken ct)
    {
        var certificate = await certificateService.GetByIdAsync(id, ct);
        return certificate is null ? NotFound() : Ok(certificate);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<CertificateResponse>> Create(
        [FromBody] CreateCertificateRequest request,
        CancellationToken ct)
    {
        var certificate = await certificateService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = certificate.Id }, certificate);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<CertificateResponse>> Update(
        Guid id,
        [FromBody] UpdateCertificateRequest request,
        CancellationToken ct)
    {
        var certificate = await certificateService.UpdateAsync(id, request, ct);
        return certificate is null ? NotFound() : Ok(certificate);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await certificateService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
