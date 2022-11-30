using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Fixtures;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class CategoryRepositoryTests : IClassFixture<UnitOfWorkFixture>
{
    private readonly CategoryRepository _CategoryRepository;
    private readonly SciMaterialsContext _Context;

    public CategoryRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<CategoryRepository>>();
        _CategoryRepository = new CategoryRepository(_Context, logger.Object);
    }

    #region GetAll

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expected_state = EntityState.Detached;

        //act
        var categories = _CategoryRepository.GetAll();
        var count = categories.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(categories[0]).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetAll_Tracking_ItShould_contains_category_1()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var categories = _CategoryRepository.GetAll();
        var count = categories.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(categories[0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Detached;

        //act
        var categories = await _CategoryRepository.GetAllAsync();
        var count = categories.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(categories[0]).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetAllAsync_Tracking_ItShould_contains_category_1()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var categories = await _CategoryRepository.GetAllAsync();
        var count = categories.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(categories[0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void AddAsync_ItShould_contains_category_increase_by_1()
    {
        //arrange
        var expected = (await _CategoryRepository.GetAllAsync()).Count + 1;
        var category = CategoryHelper.GetOne();

        //act
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        var categories = await _CategoryRepository.GetAllAsync();
        var count = categories.Count;

        var category_db = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(category.CreatedAt, category_db.CreatedAt);
        Assert.Equal(category.Description, category_db.Description);
        Assert.Equal(category.Name, category_db.Name);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Add_ItShould_contains_category_3()
    {
        //arrange
        var expected = _CategoryRepository.GetAll().Count + 1;
        var category = CategoryHelper.GetOne();

        //act
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        var categories = _CategoryRepository.GetAll();
        var count = categories.Count;

        var category_db = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(category.CreatedAt, category_db.CreatedAt);
        Assert.Equal(category.Description, category_db.Description);
        Assert.Equal(category.Name, category_db.Name);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();
        var expected = (await _CategoryRepository.GetAllAsync()).Count - 1;

        //act
        await _CategoryRepository.DeleteAsync(category.Id);
        await _Context.SaveChangesAsync();

        var categories = await _CategoryRepository.GetAllAsync();
        var count      = categories.Count;

        var removed_category = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_category);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();
        var expected = _CategoryRepository.GetAll().Count - 1;

        //act
        _CategoryRepository.Delete(category.Id);
        _Context.SaveChanges();

        var categories = _CategoryRepository.GetAll();
        var count = categories.Count;

        var removed_category = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_category);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _CategoryRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _CategoryRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        var          expected_created_at  = DateTime.Now.AddDays(1);
        const string expected_description = "new category description";
        const string expected_name        = "new category name";

        var category = CategoryHelper.GetOne();
        await _CategoryRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        category.CreatedAt = expected_created_at;
        category.Description = expected_description;
        category.Name = expected_name;
        await _CategoryRepository.UpdateAsync(category);
        await _Context.SaveChangesAsync();

        var category_db = await _CategoryRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(category.Id, category_db!.Id);
        Assert.Equal(category.Name, expected_name);
        Assert.Equal(category.Description, expected_description);
        Assert.Equal(category.CreatedAt, expected_created_at);
    }

    [Fact]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void Update_ItShould_properties_updated()
    {
        _CategoryRepository.NoTracking = false;

        //arrange
        var          expected_created_at  = DateTime.Now.AddDays(1);
        const string expected_description = "new category description";
        const string expected_name        = "new category name";

        var category = CategoryHelper.GetOne();
        _CategoryRepository.Add(category);
        _Context.SaveChanges();

        //act
        category.CreatedAt = expected_created_at;
        category.Description = expected_description;
        category.Name = expected_name;
        _CategoryRepository.Update(category);
        _Context.SaveChanges();

        var category_db = _CategoryRepository.GetById(category.Id);

        //assert
        Assert.Equal(category.Id, category_db!.Id);
        Assert.Equal(category.Name, expected_name);
        Assert.Equal(category.Description, expected_description);
        Assert.Equal(category.CreatedAt, expected_created_at);
    }

    #endregion

    #region GetByHash

    [Fact(Skip = "Не реализован метод")]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByHashAsync_ItShould_null()
    {
        //arrange

        //act
        var category_db = _CategoryRepository.GetByHashAsync(string.Empty);

        //assert
        Assert.Null(category_db);
    }

    [Fact(Skip = "Не реализован метод")]
    [Trait("CategoryRepositoryTests", nameof(Category))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var category_db = _CategoryRepository.GetByHash(string.Empty);

        //assert
        Assert.Null(category_db);
    }

    #endregion
}