using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.Tags;

public class TagService : ApiServiceBase, ITagService
{
    public TagService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<TagService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetTagResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Tag>().GetAllAsync();
        var result = _Mapper.Map<List<GetTagResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetTagResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Tag>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Tag>().GetCountAsync();
        var result = _Mapper.Map<List<GetTagResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetTagResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Tag>().GetByIdAsync(id) is not { } Tag)
        {
            return LoggedError<GetTagResponse>(
                Errors.Api.Tag.NotFound,
                "Tag with ID {id} not found",
                id);
        }

        var result = _Mapper.Map<GetTagResponse>(Tag);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddTagRequest request, CancellationToken Cancel = default)
    {
        var Tag = _Mapper.Map<Tag>(request);
        await Database.GetRepository<Tag>().AddAsync(Tag);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Tag>().GetByIdAsync(request.Id) is not { } existedTag)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.NotFound,
                "Tag {name} not found",
                request.Name);
        }

        var Tag = _Mapper.Map(request, existedTag);
        await Database.GetRepository<Tag>().UpdateAsync(Tag);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Tag>().GetByIdAsync(id) is not { } Tag)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.NotFound,
                "Tag with {id} not found",
                id);
        }

        await Database.GetRepository<Tag>().DeleteAsync(Tag);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Tag.Delete,
                "Tag with {id} update error",
                id);
        }

        return id;
    }
}
