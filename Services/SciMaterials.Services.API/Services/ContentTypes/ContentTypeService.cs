using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.ContentTypes;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.ContentTypes;

public class ContentTypeService : ApiServiceBase, IContentTypeService
{
    public ContentTypeService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<ContentTypeService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetContentTypeResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<ContentType>().GetAllAsync();
        var result = _Mapper.Map<List<GetContentTypeResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetContentTypeResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<ContentType>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<ContentType>().GetCountAsync();
        var result = _Mapper.Map<List<GetContentTypeResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetContentTypeResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<ContentType>().GetByIdAsync(id) is not { } ContentType)
        {
            return LoggedError<GetContentTypeResponse>(
                Errors.Api.ContentType.NotFound,
                "ContentType with ID {id} not found",
                id);
        }

        var result = _Mapper.Map<GetContentTypeResponse>(ContentType);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddContentTypeRequest request, CancellationToken Cancel = default)
    {
        var ContentType = _Mapper.Map<ContentType>(request);
        await Database.GetRepository<ContentType>().AddAsync(ContentType);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<ContentType>().GetByIdAsync(request.Id) is not { } existedContentType)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.NotFound,
                "ContentType {name} not found",
                request.Name);
        }

        var ContentType = _Mapper.Map(request, existedContentType);
        await Database.GetRepository<ContentType>().UpdateAsync(ContentType);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<ContentType>().GetByIdAsync(id) is not { } ContentType)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.NotFound,
                "ContentType with {id} not found",
                id);
        }

        await Database.GetRepository<ContentType>().DeleteAsync(ContentType);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.ContentType.Delete,
                "ContentType with {id} update error",
                id);
        }

        return id;
    }
}
