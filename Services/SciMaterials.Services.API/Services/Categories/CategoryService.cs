using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.Services.API.Services.Categories;

public class CategoryService : ApiServiceBase, ICategoryService
{
    public CategoryService(IUnitOfWork<SciMaterialsContext> Database, IMapper mapper, ILogger<CategoryService> logger)
        : base(Database, mapper, logger) { }

    public async Task<Result<IEnumerable<GetCategoryResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Category>().GetAllAsync();
        var result = _Mapper.Map<List<GetCategoryResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCategoryResponse>> GetPageAsync(int PageNumber, int PageSize, CancellationToken Cancel = default)
    {
        var categories = await Database.GetRepository<Category>().GetPageAsync(PageNumber, PageSize);
        var totalCount = await Database.GetRepository<Category>().GetCountAsync();
        var result = _Mapper.Map<List<GetCategoryResponse>>(categories);
        return (result, totalCount);
    }

    public async Task<Result<GetCategoryResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await Database.GetRepository<Category>().GetByIdAsync(id) is not { } Category)
        {
            return LoggedError<GetCategoryResponse>(
                Errors.Api.Category.NotFound,
                "Category with ID {id} not found",
                id);
        }

        var result = _Mapper.Map<GetCategoryResponse>(Category);
        return result;
    }

    public async Task<Result<CategoryTree>> GetTreeAsync(Guid? id, CancellationToken Cancel = default)
    {
        var repository = Database.GetRepository<Category>() as ICategoryRepository;

        string name = "root";

        if (id.HasValue)
        {
            Category? category = await repository.GetByIdAsync(id.Value);
            if (category is not { })
            {
                return LoggedError<CategoryTree>(
                    Errors.Api.Category.NotFound,
                    "Category with ID {id} is not found",
                    id);
            }
            name = category.Name;
        }


        var categories = await repository.GetByParentIdAsync(id);
        var subCategoriesInfo = categories.Select(c => new CategoryTreeInfo(c.Id, c.Name));

        var result = new CategoryTree(id, name, subCategoriesInfo);
        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddCategoryRequest request, CancellationToken Cancel = default)
    {
        var Category = _Mapper.Map<Category>(request);
        await Database.GetRepository<Category>().AddAsync(Category);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Category>().GetByIdAsync(request.Id) is not { } existedCategory)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.NotFound,
                "Category {name} not found",
                request.Name);
        }

        var Category = _Mapper.Map(request, existedCategory);
        await Database.GetRepository<Category>().UpdateAsync(Category);

        if (await Database.SaveContextAsync() == 0)
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
        if (await Database.GetRepository<Category>().GetByIdAsync(id) is not { } Category)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.NotFound,
            "Category with {id} not found",
                id);
        }

        await Database.GetRepository<Category>().DeleteAsync(Category);

        if (await Database.SaveContextAsync() == 0)
        {
            return LoggedError<Guid>(
                Errors.Api.Category.Delete,
                "Category with {id} update error",
                id);
        }

        return id;
    }
}