using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.Services.API.Services.Tags;

public class TagService : ITagService
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TagService> _logger;

    public TagService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<TagService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetTagResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Tag>().GetAllAsync();
        var result = _mapper.Map<List<GetTagResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetTagResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Tag>().GetPageAsync(pageNumber, pageSize);
        var result = _mapper.Map<List<GetTagResponse>>(categories);
        return await PageResult<GetTagResponse>.SuccessAsync(result);
    }

    public async Task<Result<GetTagResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is { } tag)
            return _mapper.Map<GetTagResponse>(tag);

        return await Result<GetTagResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddTagRequest request, CancellationToken Cancel = default)
    {
        var tag = _mapper.Map<Tag>(request);
        await _unitOfWork.GetRepository<Tag>().AddAsync(tag);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(tag.Id, "Tag created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditTagRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(request.Id) is not { } existedTag)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {request.Id} not found");

        var tag = _mapper.Map(request, existedTag);
        await _unitOfWork.GetRepository<Tag>().UpdateAsync(tag);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(tag.Id, "Tag updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is not { } tag)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Tag with ID {id} not found");

        await _unitOfWork.GetRepository<Tag>().DeleteAsync(tag);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Tag with ID {tag.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
