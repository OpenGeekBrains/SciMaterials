using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts;

namespace SciMaterials.Services.API.Services.Comments;

public class CommentService : ApiServiceBase, ICommentService
{
    public CommentService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<CommentService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetCommentResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Comment>().GetAllAsync();
        var result = _mapper.Map<List<GetCommentResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCommentResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Comment>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Comment>().GetCountAsync();
        var result = _mapper.Map<List<GetCommentResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetCommentResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(id) is not { } Comment)
        {
            return LoggedError<GetCommentResponse>(
                Errors.Api.Comment.NotFound,
                "Comment with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetCommentResponse>(Comment);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddCommentRequest request, CancellationToken Cancel = default)
    {
        var Comment = _mapper.Map<Comment>(request);
        await _unitOfWork.GetRepository<Comment>().AddAsync(Comment);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.Add,
                "Comment of user{authorId} for resource {resourceId} add error",
                request.AuthorId, request.ResourceId);
        }

        return Comment.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditCommentRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(request.Id) is not { } existedComment)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.NotFound,
                "Comment with {id} not found",
                request.Id);
        }

        var Comment = _mapper.Map(request, existedComment);
        await _unitOfWork.GetRepository<Comment>().UpdateAsync(Comment);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.Update,
                "Comment with {id} update error",
                request.Id);
        }

        return Comment.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(id) is not { } Comment)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.NotFound,
                "Comment with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<Comment>().DeleteAsync(Comment);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.Delete,
                "Comment with {id} update error",
                id);
        }

        return id;
    }
}
