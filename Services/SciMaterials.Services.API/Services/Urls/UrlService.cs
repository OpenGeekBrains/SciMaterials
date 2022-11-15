using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts;

namespace SciMaterials.Services.API.Services.Urls;

public class UrlService : ApiServiceBase, IUrlService
{
    public UrlService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<UrlService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetUrlResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Url>().GetAllAsync();
        var result = _mapper.Map<List<GetUrlResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetUrlResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Url>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Url>().GetCountAsync();
        var result = _mapper.Map<List<GetUrlResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetUrlResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(id) is not { } Url)
        {
            return LoggedError<GetUrlResponse>(
                Errors.Api.Url.NotFound,
                "Url with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetUrlResponse>(Url);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddUrlRequest request, CancellationToken Cancel = default)
    {
        var Url = _mapper.Map<Url>(request);
        await _unitOfWork.GetRepository<Url>().AddAsync(Url);

        if (await _unitOfWork.SaveContextAsync() == 0)
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
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(request.Id) is not { } existedUrl)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.NotFound,
                "Url {name} not found",
                request.Name);
        }

        var Url = _mapper.Map(request, existedUrl);
        await _unitOfWork.GetRepository<Url>().UpdateAsync(Url);

        if (await _unitOfWork.SaveContextAsync() == 0)
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
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(id) is not { } Url)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.NotFound,
                "Url with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<Url>().DeleteAsync(Url);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Url.Delete,
                "Url with {id} update error",
                id);
        }

        return id;
    }
}
