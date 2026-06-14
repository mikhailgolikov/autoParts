using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.Companies;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController(CompanyService companyService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<CompanyResponse>> Get(CancellationToken ct) =>
        Ok(await companyService.GetAsync(ct));

    [HttpPut]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<CompanyResponse>> Update(
        [FromBody] UpdateCompanyRequest request,
        CancellationToken ct) =>
        Ok(await companyService.UpdateAsync(request, ct));
}
