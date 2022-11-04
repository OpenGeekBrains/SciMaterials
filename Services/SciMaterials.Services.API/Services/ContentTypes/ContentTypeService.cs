using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.Services.API.Services.ContentTypes;

public class ContentTypeService : IContentTypeService
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ContentTypeService> _logger;

    public ContentTypeService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<ContentTypeService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetContentTypeResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<ContentType>().GetAllAsync();
        var result = _mapper.Map<List<GetContentTypeResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetContentTypeResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<ContentType>().GetPageAsync(pageNumber, pageSize);
        var result = _mapper.Map<List<GetContentTypeResponse>>(categories);
        return await PageResult<GetContentTypeResponse>.SuccessAsync(result);
    }

    public async Task<Result<GetContentTypeResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is { } contentType)
            return _mapper.Map<GetContentTypeResponse>(contentType);

        return await Result<GetContentTypeResponse>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddContentTypeRequest request, CancellationToken Cancel = default)
    {
        var contentType = _mapper.Map<ContentType>(request);
        await _unitOfWork.GetRepository<ContentType>().AddAsync(contentType);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(contentType.Id, "ContentType created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditContentTypeRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(request.Id) is not { } existedCommentType)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {request.Id} not found");

        var contentType = _mapper.Map(request, existedCommentType);
        await _unitOfWork.GetRepository<ContentType>().UpdateAsync(contentType);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(contentType.Id, "ContentType updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is not { } contentType)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"ContentType with ID {id} not found");

        await _unitOfWork.GetRepository<ContentType>().DeleteAsync(contentType);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"ContentType with ID {contentType.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
