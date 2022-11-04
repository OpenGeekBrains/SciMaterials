using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Urls;

namespace SciMaterials.Services.API.Services.Urls;

public class UrlService : IUrlService
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UrlService> _logger;

    public UrlService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<UrlService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetUrlResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Url>().GetAllAsync();
        var result = _mapper.Map<List<GetUrlResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetUrlResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Url>().GetPageAsync(pageNumber, pageSize);
        var result = _mapper.Map<List<GetUrlResponse>>(categories);
        return await PageResult<GetUrlResponse>.SuccessAsync(result);
    }

    public async Task<Result<GetUrlResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(id) is { } author)
            return _mapper.Map<GetUrlResponse>(author);

        return await Result<GetUrlResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Url with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddUrlRequest request, CancellationToken cancellationToken = default)
    {
        var author = _mapper.Map<Url>(request);
        await _unitOfWork.GetRepository<Url>().AddAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Url created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(request.Id) is not { } existedUrl)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Url with ID {request.Id} not found");

        var author = _mapper.Map(request, existedUrl);
        await _unitOfWork.GetRepository<Url>().UpdateAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(author.Id, "Url updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Url>().GetByIdAsync(id) is not { } author)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Url with ID {id} not found");

        await _unitOfWork.GetRepository<Url>().DeleteAsync(author);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Url with ID {author.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
