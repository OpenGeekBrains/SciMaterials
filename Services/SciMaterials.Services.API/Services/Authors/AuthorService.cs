using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Authors;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts;

namespace SciMaterials.Services.API.Services.Authors;

public class AuthorService : ApiServiceBase, IAuthorService
{
    public AuthorService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<AuthorService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetAuthorResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Author>().GetAllAsync();
        var result = _mapper.Map<List<GetAuthorResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetAuthorResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Author>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Author>().GetCountAsync();
        var result = _mapper.Map<List<GetAuthorResponse>>(categories);
        return (result, totalCount); 
    }

    public async Task<Result<GetAuthorResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(id) is not { } author)
        {
            return LoggedError<GetAuthorResponse>(
                Errors.Api.Author.NotFound,
                "Author with ID {id} not found",
                id);
        }
                    
        var result = _mapper.Map<GetAuthorResponse>(author);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddAuthorRequest request, CancellationToken Cancel = default)
    {
        var author = _mapper.Map<Author>(request);
        await _unitOfWork.GetRepository<Author>().AddAsync(author);

        if (await _unitOfWork.SaveContextAsync() == 0)
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
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(request.Id) is not { } existedAuthor)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.NotFound,
                "Author {name} not found",
                request.Name);
        }

        var author = _mapper.Map(request, existedAuthor);
        await _unitOfWork.GetRepository<Author>().UpdateAsync(author);

        if (await _unitOfWork.SaveContextAsync() == 0)
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
        if (await _unitOfWork.GetRepository<Author>().GetByIdAsync(id) is not { } author)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.NotFound,
                "Author with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<Author>().DeleteAsync(author);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Author.Delete,
                "Author with {id} update error",
                id);
        }
            
        return id;
    }
}