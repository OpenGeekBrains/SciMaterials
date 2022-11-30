using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.Services.API.Services.Authors;

public class AuthorService : ApiServiceBase, IAuthorService
{
    public AuthorService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<AuthorService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetAuthorResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Author>().GetAllAsync();
        var result = _Mapper.Map<List<GetAuthorResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetAuthorResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Author>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Author>().GetCountAsync();
        var result = _Mapper.Map<List<GetAuthorResponse>>(categories);
        return (result, totalCount); 
    }

    public async Task<Result<GetAuthorResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Author>().GetByIdAsync(id) is not { } author)
        {
            return LoggedError<GetAuthorResponse>(
                Errors.Api.Author.NotFound,
                "Author with ID {id} not found",
                id);
        }
                    
        var result = _Mapper.Map<GetAuthorResponse>(author);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddAuthorRequest request, CancellationToken Cancel = default)
    {
        var author = _Mapper.Map<Author>(request);
        await Database.GetRepository<Author>().AddAsync(author);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.Add,
                "Author {name} add error",
                request.Name);
        }

        return author.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditAuthorRequest request, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Author>().GetByIdAsync(request.Id) is not { } existedAuthor)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.NotFound,
                "Author {name} not found",
                request.Name);
        }

        var author = _Mapper.Map(request, existedAuthor);
        await Database.GetRepository<Author>().UpdateAsync(author);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.Update,
                "Author {name} update error",
                request.Name);
        }

        return author.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Author>().GetByIdAsync(id) is not { } author)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.NotFound,
                "Author with {id} not found",
                id);
        }

        await Database.GetRepository<Author>().DeleteAsync(author);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.Delete,
                "Author with {id} update error",
                id);
        }
            
        return id;
    }
}