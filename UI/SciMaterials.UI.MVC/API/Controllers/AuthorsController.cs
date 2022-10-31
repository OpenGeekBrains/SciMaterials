using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with authors. </summary>
[ApiController]
[Route(WebApiRoute.Authors)]
public class AuthorsController : ApiBaseController<AuthorsController>
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary> Get All Authors. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetAuthorResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var сategories = await _authorService.GetAllAsync();
        return Ok(сategories);
    }

    /// <summary> Get Author by Id. </summary>
    /// <param name="id"> Author Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetAuthorResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var products = await _authorService.GetByIdAsync(id);
        return Ok(products);
    }

    /// <summary> Add a Author. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddAuthorRequest request)
    {
        var result = await _authorService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a Author. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditAuthorRequest request)
    {
        var result = await _authorService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a Author. </summary>
    /// <param name="id"> Author Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _authorService.DeleteAsync(id);
        return Ok(result);
    }
}