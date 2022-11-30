using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.Urls;

public class UrlService : ApiServiceBase, IUrlService
{
    public UrlService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<UrlService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetUrlResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Url>().GetAllAsync();
        var result = _Mapper.Map<List<GetUrlResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetUrlResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Url>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Url>().GetCountAsync();
        var result = _Mapper.Map<List<GetUrlResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetUrlResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Url>().GetByIdAsync(id) is not { } Url)
        {
            return LoggedError<GetUrlResponse>(
                Errors.Api.Url.NotFound,
                "Url with ID {id} not found",
                id);
        }

        var result = _Mapper.Map<GetUrlResponse>(Url);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddUrlRequest request, CancellationToken Cancel = default)
    {
        var Url = _Mapper.Map<Url>(request);
        await Database.GetRepository<Url>().AddAsync(Url);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.Add,
                "Url {name} add error",
                request.Name);
        }

        return Url.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditUrlRequest request, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Url>().GetByIdAsync(request.Id) is not { } existedUrl)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.NotFound,
                "Url {name} not found",
                request.Name);
        }

        var Url = _Mapper.Map(request, existedUrl);
        await Database.GetRepository<Url>().UpdateAsync(Url);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.Update,
                "Url {name} update error",
                request.Name);
        }

        return Url.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Url>().GetByIdAsync(id) is not { } Url)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.NotFound,
                "Url with {id} not found",
                id);
        }

        await Database.GetRepository<Url>().DeleteAsync(Url);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.Delete,
                "Url with {id} update error",
                id);
        }

        return id;
    }
}
