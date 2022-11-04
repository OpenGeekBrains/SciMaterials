using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with comments. </summary>
[ApiController]
[Route(WebApiRoute.Comments)]
public class CommentsController : ApiBaseController<CommentsController>
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary> Get All Comments. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetCommentResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var сategories = await _commentService.GetAllAsync();
        return Ok(сategories);
    }

    /// <summary> Get Comment by Id. </summary>
    /// <param name="id"> Comment Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetCommentResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var products = await _commentService.GetByIdAsync(id);
        return Ok(products);
    }

    /// <summary> Add a Comment. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddCommentRequest request)
    {
        var result = await _commentService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a Comment. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditCommentRequest request)
    {
        var result = await _commentService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a Comment. </summary>
    /// <param name="id"> Comment Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _commentService.DeleteAsync(id);
        return Ok(result);
    }
}