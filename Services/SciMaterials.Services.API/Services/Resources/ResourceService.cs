using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.Contracts.API.Services.Resources;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.Resources;

public class ResourceService : ApiServiceBase, IResourceService
{
    public ResourceService(IUnitOfWork<SciMaterialsContext> Database, IMapper Mapper, ILogger<ResourceService> Logger)
        : base(Database, Mapper, Logger) { }

    public async Task<Result<IEnumerable<GetResourceResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var resources = await Database.GetRepository<Resource>().GetAllAsync();
        var result = _Mapper.Map<List<GetResourceResponse>>(resources);
        return result;
    }

    public async Task<PageResult<GetResourceResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var resources = await Database.GetRepository<Resource>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Resource>().GetCountAsync();
        var result = _Mapper.Map<List<GetResourceResponse>>(resources);
        return PageResult<GetResourceResponse>.Success(result, totalCount, PageNumber, PageSize);
    }

    public async Task<Result<GetResourceResponse>> GetByIdAsync(Guid Id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Resource>().GetByIdAsync(Id) is not { } resource)
        {
            return LoggedError<GetResourceResponse>(
                Errors.Api.Resource.NotFound,
                "Resource with ID {id} not found",
                Id);
        }

        var result = _Mapper.Map<GetResourceResponse>(resource);
        return result;
    }
}
