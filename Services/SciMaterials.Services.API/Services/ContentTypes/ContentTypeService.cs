using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.Contracts;
using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.Services.API.Services.ContentTypes;

public class ContentTypeService : ApiServiceBase, IContentTypeService
{
    public ContentTypeService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<ContentTypeService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetContentTypeResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<ContentType>().GetAllAsync();
        var result = _mapper.Map<List<GetContentTypeResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetContentTypeResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<ContentType>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<ContentType>().GetCountAsync();
        var result = _mapper.Map<List<GetContentTypeResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetContentTypeResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is not { } ContentType)
        {
            return LoggedError<GetContentTypeResponse>(
                Errors.Api.ContentType.NotFound,
                "ContentType with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetContentTypeResponse>(ContentType);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddContentTypeRequest request, CancellationToken Cancel = default)
    {
        var ContentType = _mapper.Map<ContentType>(request);
        await _unitOfWork.GetRepository<ContentType>().AddAsync(ContentType);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.Add,
                "ContentType {name} add error",
                request.Name);
        }

        return ContentType.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditContentTypeRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(request.Id) is not { } existedContentType)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.NotFound,
                "ContentType {name} not found",
                request.Name);
        }

        var ContentType = _mapper.Map(request, existedContentType);
        await _unitOfWork.GetRepository<ContentType>().UpdateAsync(ContentType);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.Update,
                "ContentType {name} update error",
                request.Name);
        }

        return ContentType.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<ContentType>().GetByIdAsync(id) is not { } ContentType)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.NotFound,
                "ContentType with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<ContentType>().DeleteAsync(ContentType);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.Delete,
                "ContentType with {id} update error",
                id);
        }

        return id;
    }
}
