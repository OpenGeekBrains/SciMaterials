using SciMaterials.DAL.UnitOfWork;
using SciMaterials.DAL.Contexts;
using AutoMapper;
using Category = SciMaterials.DAL.Models.Category;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.Services.API.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetCategoryResponse>>> GetAllAsync(CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
        var result = _mapper.Map<List<GetCategoryResponse>>(categories);
        return result;
    }

    public async Task<PageResult<GetCategoryResponse>> GetPageAsync(int pageNumber, int pageSize, CancellationToken Cancel = default)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetPageAsync(pageNumber, pageSize);
        var result = _mapper.Map<List<GetCategoryResponse>>(categories);
        return await PageResult<GetCategoryResponse>.SuccessAsync(result);
    }

    public async Task<Result<GetCategoryResponse>> GetByIdAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(id) is { } category)
            return _mapper.Map<GetCategoryResponse>(category);

        return await Result<GetCategoryResponse>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {id} not found");
    }

    public async Task<Result<Guid>> AddAsync(AddCategoryRequest request, CancellationToken Cancel = default)
    {
        var category = _mapper.Map<Category>(request);
        category.CreatedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Category>().AddAsync(category);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(category.Id, "Category created");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> EditAsync(EditCategoryRequest request, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.Id) is not { } existedCategory)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {request.Id} not found");

        var category = _mapper.Map(request, existedCategory);
        await _unitOfWork.GetRepository<Category>().UpdateAsync(category);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync(category.Id, "Category updated");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken Cancel = default)
    {
        if (await _unitOfWork.GetRepository<Category>().GetByIdAsync(id) is not { } category)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Category with ID {id} not found");

        await _unitOfWork.GetRepository<Category>().DeleteAsync(category);

        if (await _unitOfWork.SaveContextAsync() > 0)
            return await Result<Guid>.SuccessAsync($"Category with ID {category.Id} deleted");

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");
    }
}
