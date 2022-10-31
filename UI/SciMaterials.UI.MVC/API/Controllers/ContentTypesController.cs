using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with authors. </summary>
[ApiController]
[Route(WebApiRoute.ContentTypes)]
public class ContentTypesController : ApiBaseController<ContentTypesController>
{
    private readonly IContentTypeService _authorService;

    public ContentTypesController(IContentTypeService authorService)
    {
        _authorService = authorService;
    }

    /// <summary> Get All ContentTypes. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetContentTypeResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var сategories = await _authorService.GetAllAsync();
        return Ok(сategories);
    }

    /// <summary> Get ContentType by Id. </summary>
    /// <param name="id"> ContentType Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetContentTypeResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var products = await _authorService.GetByIdAsync(id);
        return Ok(products);
    }

    /// <summary> Add a ContentType. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddContentTypeRequest request)
    {
        var result = await _authorService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a ContentType. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditContentTypeRequest request)
    {
        var result = await _authorService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a ContentType. </summary>
    /// <param name="id"> ContentType Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _authorService.DeleteAsync(id);
        return Ok(result);
    }
}