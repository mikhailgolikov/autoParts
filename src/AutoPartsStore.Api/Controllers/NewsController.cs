using AutoPartsStore.Api.Auth;
using AutoPartsStore.Api.Contracts.News;
using AutoPartsStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Api.Controllers;

[ApiController]
[Route("api/news")]
public class NewsController(NewsService newsService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<NewsResponse>>> GetAll(CancellationToken ct) =>
        Ok(await newsService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<NewsResponse>> GetById(Guid id, CancellationToken ct)
    {
        var news = await newsService.GetByIdAsync(id, ct);
        return news is null ? NotFound() : Ok(news);
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<NewsResponse>> Create(
        [FromBody] CreateNewsRequest request,
        CancellationToken ct)
    {
        var news = await newsService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.Creator}")]
    public async Task<ActionResult<NewsResponse>> Update(
        Guid id,
        [FromBody] UpdateNewsRequest request,
        CancellationToken ct)
    {
        var news = await newsService.UpdateAsync(id, request, ct);
        return news is null ? NotFound() : Ok(news);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await newsService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
