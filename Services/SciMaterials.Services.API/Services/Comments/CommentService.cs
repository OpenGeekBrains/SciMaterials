using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.Comments;

public class CommentService : ApiServiceBase, ICommentService
{
    public CommentService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<CommentService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetCommentResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Comment>().GetAllAsync();
        var result = _Mapper.Map<List<GetCommentResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCommentResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Comment>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Comment>().GetCountAsync();
        var result = _Mapper.Map<List<GetCommentResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetCommentResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Comment>().GetByIdAsync(id) is not { } Comment)
        {
            return LoggedError<GetCommentResponse>(
                Errors.Api.Comment.NotFound,
                "Comment with ID {id} not found",
                id);
        }

        var result = _Mapper.Map<GetCommentResponse>(Comment);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddCommentRequest request, CancellationToken Cancel = default)
    {
        var Comment = _Mapper.Map<Comment>(request);
        await Database.GetRepository<Comment>().AddAsync(Comment);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Comment>().GetByIdAsync(request.Id) is not { } existedComment)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.NotFound,
                "Comment with {id} not found",
                request.Id);
        }

        var Comment = _Mapper.Map(request, existedComment);
        await Database.GetRepository<Comment>().UpdateAsync(Comment);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Comment>().GetByIdAsync(id) is not { } Comment)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.NotFound,
                "Comment with {id} not found",
                id);
        }

        await Database.GetRepository<Comment>().DeleteAsync(Comment);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Comment.Delete,
                "Comment with {id} update error",
                id);
        }

        return id;
    }
}
