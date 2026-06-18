using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.ForSuppliersPage;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers
{
    [ApiController]
    [Route("api/for-suppliers")]
    public class ForSuppliersPageController(ForSuppliersPageService service) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ForSuppliersPageResponse>> Get(CancellationToken ct) =>
        Ok(await service.GetAsync(ct));

        [HttpPut]
        [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
        public async Task<ActionResult<ForSuppliersPageResponse>> Update(
            [FromBody] UpdateForSuppliersPageRequest request,
            CancellationToken ct) =>
            Ok(await service.UpdateAsync(request, ct));
    }
}
