using Microsoft.AspNetCore.Mvc;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.UrlShortener.Contracts;

namespace SciMaterials.Materials.Api.Controllers;

/// <summary> Service for working with short links. </summary>
[ApiController]
[Route(WebApiRoute.ShortLinks)]
public class LinksController : ApiBaseController<LinksController>
{
    private readonly ILinkShortCutService _linkShortCut;

    public LinksController(ILinkShortCutService linkShortCut)
        => _linkShortCut = linkShortCut;

    [HttpGet("{hash}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string hash)
    {
        var linkResult = await _linkShortCut.GetAsync(hash, true);

        if (linkResult.Succeeded)
            return Redirect(linkResult.Data);

        return NotFound();
    }
}