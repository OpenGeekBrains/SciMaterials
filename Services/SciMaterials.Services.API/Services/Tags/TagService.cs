using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts;

namespace SciMaterials.Services.API.Services.Tags;

public class TagService : ApiServiceBase, ITagService
{
    public TagService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<TagService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetTagResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Tag>().GetAllAsync();
        var result = _mapper.Map<List<GetTagResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetTagResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Tag>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Tag>().GetCountAsync();
        var result = _mapper.Map<List<GetTagResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetTagResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is not { } Tag)
        {
            return LoggedError<GetTagResponse>(
                Errors.Api.Tag.NotFound,
                "Tag with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetTagResponse>(Tag);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddTagRequest request, CancellationToken Cancel = default)
    {
        var Tag = _mapper.Map<Tag>(request);
        await _unitOfWork.GetRepository<Tag>().AddAsync(Tag);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.Add,
                "Tag {name} add error",
                request.Name);
        }

        return Tag.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditTagRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(request.Id) is not { } existedTag)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.NotFound,
                "Tag {name} not found",
                request.Name);
        }

        var Tag = _mapper.Map(request, existedTag);
        await _unitOfWork.GetRepository<Tag>().UpdateAsync(Tag);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.Update,
                "Tag {name} update error",
                request.Name);
        }

        return Tag.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Tag>().GetByIdAsync(id) is not { } Tag)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.NotFound,
                "Tag with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<Tag>().DeleteAsync(Tag);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.Delete,
                "Tag with {id} update error",
                id);
        }

        return id;
    }
}
