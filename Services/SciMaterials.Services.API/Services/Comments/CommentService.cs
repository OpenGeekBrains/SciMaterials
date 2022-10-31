using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Comments;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.Services.API.Services.Comments;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CommentService> _logger;

    public CommentService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<CommentService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetCommentResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Comment>().GetAllAsync();
        var result = _mapper.Map<List<GetCommentResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCommentResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Comment>().GetPageAsync(pageNumber, pageSize);
        var result = _mapper.Map<List<GetCommentResponse>>(categories);
        return await PageResult<GetCommentResponse>.SuccessAsync(result);
    }

    public async Task<Result<GetCommentResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(id) is { } comment)
            return _mapper.Map<GetCommentResponse>(comment);

        return await Result<GetCommentResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Comment with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddCommentRequest request, CancellationToken Cancel = default)
    {
        var comment = _mapper.Map<Comment>(request);
        comment.CreatedAt = DateTime.Now;
        if (await VerifyRelatedData(comment) is { Succeeded: false } verifyResult)
            return verifyResult;

        await _unitOfWork.GetRepository<Comment>().AddAsync(comment);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(comment.Id, "Comment created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditCommentRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(request.Id) is not { } existedComment)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Comment with ID {request.Id} not found");


        var comment = _mapper.Map(request, existedComment);
        if (await VerifyRelatedData(comment) is { Succeeded: false } verifyResult)
            return verifyResult;

        await _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(comment.Id, "Comment updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    private async Task<Result<Guid>> VerifyRelatedData(Comment comment)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(comment.AuthorId) is not { } author)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Comment aгthor with ID {comment.FileId} not found");
        comment.Author = author;

        if (comment.FileId.HasValue && await _unitOfWork.GetRepository<DAL.Models.File>().GetByIdAsync(comment.FileId.Value) is not { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Comment file with ID {comment.FileId} not found");

        if (comment.FileGroupId.HasValue && await _unitOfWork.GetRepository<FileGroup>().GetByIdAsync(comment.FileGroupId.Value) is not { })
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Comment file group with ID {comment.FileGroupId} not found");

        return await Result<Guid>.SuccessAsync();
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Comment>().GetByIdAsync(id) is not { } comment)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Comment with ID {id} not found");

        await _unitOfWork.GetRepository<Comment>().DeleteAsync(comment);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Comment with ID {comment.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
