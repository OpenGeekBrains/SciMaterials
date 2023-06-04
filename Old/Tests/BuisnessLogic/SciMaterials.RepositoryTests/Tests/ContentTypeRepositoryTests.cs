using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class ContentTypeRepositoryTests
{
    private readonly ContentTypeRepository _ContentTypeRepository;
    private readonly SciMaterialsContext _Context;

    public ContentTypeRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<ContentTypeRepository>>();
        _ContentTypeRepository = new ContentTypeRepository(_Context, logger.Object);
    }


    #region GetAll

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_AsNoTracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expected_state = EntityState.Detached;

        //act
        var content_types = _ContentTypeRepository.GetAll();
        var count = content_types.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(content_types[0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_Tracking_ItShould_contains_contentType_1()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var content_types = _ContentTypeRepository.GetAll();
        var count = content_types.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(content_types[0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_contentType_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Detached;

        //act
        var content_types = await _ContentTypeRepository.GetAllAsync();
        var count = content_types.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(content_types[0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_Tracking_ItShould_contains_contentType_1()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var content_types = await _ContentTypeRepository.GetAllAsync();
        var count = content_types.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(content_types[0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void AddAsync_ItShould_contains_contentType_increase_by_1()
    {
        _ContentTypeRepository.Include = true;

        //arrange
        var all = _ContentTypeRepository.GetAllAsync();
        var expected  = (await all).Count + 1;
        var category  = ContentTypeHelper.GetOne();

        //act
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        var content_types = await _ContentTypeRepository.GetAllAsync();
        var count = content_types.Count;

        var category_db = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(category.FileExtension, category_db.FileExtension);
        Assert.Equal(category.Files.First().Id, category_db.Files.First().Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Add_ItShould_contains_contentType_3()
    {
        _ContentTypeRepository.Include = true;

        //arrange
        var expected = _ContentTypeRepository.GetAll().Count + 1;
        var category = ContentTypeHelper.GetOne();

        //act
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        var content_types = _ContentTypeRepository.GetAll();
        var count = content_types.Count;

        var category_db = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(category.FileExtension, category_db.FileExtension);
        Assert.Equal(category.Files.First().Id, category_db.Files.First().Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();
        var all = await _ContentTypeRepository.GetAllAsync();
        var expected  = all.Count - 1;

        //act
        await _ContentTypeRepository.DeleteAsync(category.Id);
        await _Context.SaveChangesAsync();

        var content_types = await _ContentTypeRepository.GetAllAsync();
        var count = content_types.Count;

        var removed_content_type = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_content_type);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();
        var expected = _ContentTypeRepository.GetAll().Count - 1;

        //act
        _ContentTypeRepository.Delete(category.Id);
        _Context.SaveChanges();

        var content_types = _ContentTypeRepository.GetAll();
        var count = content_types.Count;

        var removed_content_type = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_content_type);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Id, category_db.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
        Assert.Equal(expected_state, _Context.Entry(category_db).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        var category_db = await _ContentTypeRepository.GetByNameAsync(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        var category_db = _ContentTypeRepository.GetByName(category.Name);

        //assert
        Assert.NotNull(category_db);
        Assert.Equal(category.Name, category_db.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _ContentTypeRepository.NoTracking = false;


        //arrange
        const string expected_name = "new content type name";

        var category = ContentTypeHelper.GetOne();
        await _ContentTypeRepository.AddAsync(category);
        await _Context.SaveChangesAsync();

        //act
        category.Name = expected_name;
        await _ContentTypeRepository.UpdateAsync(category);
        await _Context.SaveChangesAsync();

        var category_db = await _ContentTypeRepository.GetByIdAsync(category.Id);

        //assert
        Assert.Equal(category_db!.Id, category.Id);
        Assert.Equal(expected_name, category.Name);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Update_ItShould_properties_updated()
    {
        _ContentTypeRepository.NoTracking = false;

        //arrange
        const string expected_name = "new content type name";

        var category = ContentTypeHelper.GetOne();
        _ContentTypeRepository.Add(category);
        _Context.SaveChanges();

        //act
        category.Name = expected_name;
        _ContentTypeRepository.Update(category);
        _Context.SaveChanges();

        var category_db = _ContentTypeRepository.GetById(category.Id);

        //assert
        Assert.Equal(category.Id, category_db!.Id);
        Assert.Equal(category.Name, expected_name);
    }

    #endregion

    [Fact(Skip = "Не реализован метод")]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByHashAsync_ItShould_null()
    {
        //arrange

        //act
        var categoryDb = await _ContentTypeRepository.GetByHashAsync(String.Empty);

        //assert
        Assert.Null(categoryDb);
    }

    [Fact(Skip = "Не реализован метод")]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var categoryDb = _ContentTypeRepository.GetByHash(String.Empty);

        //assert
        Assert.Null(categoryDb);
    }
}
