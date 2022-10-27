using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Fixtures;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class CategoryRepositoryTests : IClassFixture<UnitOfWorkFixture>
{
    private ICategoryRepository _categoryRepository;
    private SciMaterialsContext _context;

    public CategoryRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _categoryRepository = new CategoryRepository(_context, logger);
    }

    #region GetAll

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_AsNoTracking_ItShould_contains_categories()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var categories = _categoryRepository.GetAll();

        //assert
        Assert.NotNull(categories);
        Assert.Equal(expecedState, _context.Entry(categories![0]).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_Tracking_ItShould_contains_categories()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var categories = _categoryRepository.GetAll(false);

        //assert
        Assert.NotNull(categories);
        Assert.Equal(expecedSstate, _context.Entry(categories![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_categories()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var categories = await _categoryRepository.GetAllAsync();

        //assert
        Assert.NotNull(categories);
        Assert.Equal(expecedSstate, _context.Entry(categories![0]).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_Tracking_ItShould_contains_categories()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var categories = await _categoryRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(categories);
        Assert.Equal(expecedSstate, _context.Entry(categories![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void AddAsync_ItShould_contains_category_increase_by_1()
    {
        //arrange
        var expected = (await _categoryRepository.GetAllAsync())!.Count + 1;
        var category = CategoryHelper.GetOne();

        //act
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        var categories = await _categoryRepository.GetAllAsync();
        var count = 0;
        if (categories is not null)
            count = categories.Count;

        var categoryDb = await _categoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.CreatedAt, categoryDb.CreatedAt);
        Assert.Equal(category.Description, categoryDb.Description);
        Assert.Equal(category.Name, categoryDb.Name);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Add_ItShould_contains_category_3()
    {
        //arrange
        var expected = _categoryRepository.GetAll()!.Count + 1;
        var category = CategoryHelper.GetOne();

        //act
        _categoryRepository.Add(category);
        _context.SaveChanges();

        var categories = _categoryRepository.GetAll();
        var count = 0;
        if (categories is not null)
            count = categories.Count;

        var categoryDb = _categoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.CreatedAt, categoryDb.CreatedAt);
        Assert.Equal(category.Description, categoryDb.Description);
        Assert.Equal(category.Name, categoryDb.Name);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();
        var expected = (await _categoryRepository.GetAllAsync())!.Count - 1;

        //act
        await _categoryRepository.DeleteAsync(category.Id);
        await _context.SaveChangesAsync();

        var categories = await _categoryRepository.GetAllAsync();
        var count      = 0;
        if (categories is not null)
            count = categories.Count;

        var removedCategory = await _categoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedCategory);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();
        var expected = _categoryRepository.GetAll()!.Count - 1;

        //act
        _categoryRepository.Delete(category.Id);
        _context.SaveChanges();

        var categories = _categoryRepository.GetAll();
        var count = 0;
        if (categories is not null)
            count = categories.Count;

        var removedCategory = _categoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedCategory);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByIdAsync(category.Id, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByIdAsync(category.Id, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetById(category.Id, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetById(category.Id, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetById(category.Id);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Id, categoryDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByNameAsync(category.Name, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetByName(category.Name, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByNameAsync(category.Name, false);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetByName(category.Name, true);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
        Assert.Equal(expecedState, _context.Entry(categoryDb).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        var categoryDb = await _categoryRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        var categoryDb = _categoryRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(categoryDb);
        Assert.Equal(category.Name, categoryDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedCreatedAt = DateTime.Now.AddDays(1);
        var expectedDescription = "new category description";
        var expectedName = "new category name";

        var category = CategoryHelper.GetOne();
        await _categoryRepository.AddAsync(category);
        await _context.SaveChangesAsync();

        //act
        category.CreatedAt = expectedCreatedAt;
        category.Description = expectedDescription;
        category.Name = expectedName;
        await _categoryRepository.UpdateAsync(category);
        await _context.SaveChangesAsync();

        var categoryDb = await _categoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, expectedName);
        Assert.Equal(category.Description, expectedDescription);
        Assert.Equal(category.CreatedAt, expectedCreatedAt);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedCreatedAt = DateTime.Now.AddDays(1);
        var expectedDescription = "new category description";
        var expectedName = "new category name";

        var category = CategoryHelper.GetOne();
        _categoryRepository.Add(category);
        _context.SaveChanges();

        //act
        category.CreatedAt = expectedCreatedAt;
        category.Description = expectedDescription;
        category.Name = expectedName;
        _categoryRepository.Update(category);
        _context.SaveChanges();

        var categoryDb = _categoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(category.Id, categoryDb!.Id);
        Assert.Equal(category.Name, expectedName);
        Assert.Equal(category.Description, expectedDescription);
        Assert.Equal(category.CreatedAt, expectedCreatedAt);
    }

    #endregion

    #region GetByHash

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByHashAsync_ItShould_null()
    {
        //arrange

        //act
        var categoryDb = _categoryRepository.GetByHashAsync(String.Empty);

        //assert
        Assert.Null(categoryDb);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var categoryDb = _categoryRepository.GetByHash(String.Empty);

        //assert
        Assert.Null(categoryDb);
    }

    #endregion
}