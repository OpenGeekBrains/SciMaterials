using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts;
using SciMaterials.DAL.Models;

namespace SciMaterials.Services.API.Services.Categories;

public class CategoryService : ApiServiceBase, ICategoryService
{
    public CategoryService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        : base(unitOfWork, mapper, logger) { }

    public async Task<Result<IEnumerable<GetCategoryResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
        var result = _mapper.Map<List<GetCategoryResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCategoryResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetPageAsync(pageNumber, pageSize);
        var totalCount = await _unitOfWork.GetRepository<Category>().GetCountAsync();
        var result = _mapper.Map<List<GetCategoryResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetCategoryResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(id) is not { } Category)
        {
            return LoggedError<GetCategoryResponse>(
                Errors.Api.Category.NotFound,
                "Category with ID {id} not found",
                id);
        }

        var result = _mapper.Map<GetCategoryResponse>(Category);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddCategoryRequest request, CancellationToken Cancel = default)
    {
        var Category = _mapper.Map<Category>(request);
        await _unitOfWork.GetRepository<Category>().AddAsync(Category);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.Add,
                "Category {name} add error",
                request.Name);
        }

        return Category.Id;
    }

    public async Task<Result<Guid>> EditAsync(EditCategoryRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.Id) is not { } existedCategory)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.NotFound,
                "Category {name} not found",
                request.Name);
        }

        var Category = _mapper.Map(request, existedCategory);
        await _unitOfWork.GetRepository<Category>().UpdateAsync(Category);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.Update,
                "Category {name} update error",
                request.Name);
        }

        return Category.Id;
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(id) is not { } Category)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.NotFound,
            "Category with {id} not found",
                id);
        }

        await _unitOfWork.GetRepository<Category>().DeleteAsync(Category);

        if (await _unitOfWork.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.Delete,
                "Category with {id} update error",
                id);
        }

        return id;
    }
}