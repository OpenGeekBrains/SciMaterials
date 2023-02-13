using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.Services.Resources;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.Contracts;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with resources. </summary>
[ApiController]
[Route(WebApiRoute.Resources)]
public class ResourcesController : ApiBaseController<ResourcesController>
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    /// <summary> Get All Resource. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetResourceResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogDebug("Get all resourses");
        var result = await _resourceService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get Resource by Id. </summary>
    /// <param name="id"> Resource Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetResourceResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        _logger.LogDebug("Get by id {id}", id);
        var result = await _resourceService.GetByIdAsync(id);
        return Ok(result);
    }
    /// <summary> Get paged resource . </summary>
    /// <param name="pageNumber"> Page number. </param>
    /// <param name="pageSize"> Page size. </param>
    [HttpGet("page/{pageNumber}/{pageSize}")]
    [ProducesDefaultResponseType(typeof(PageResult<GetResourceResponse>))]
    public async Task<IActionResult> GetPageAsync([FromRoute] int pageNumber, [FromRoute] int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            _logger.LogError("The pageNumber must be greater than 0 and pageSize must be greater than 1");
            return Ok(Result.Failure(Errors.Api.Resource.PageParametersValidationError));
        }

        _logger.LogDebug("Get paged resourses");
        var result = await _resourceService.GetPageAsync(pageNumber, pageSize);
        return Ok(result);
    }
}