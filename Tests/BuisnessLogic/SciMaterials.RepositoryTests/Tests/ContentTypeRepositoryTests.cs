using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class ContentTypeRepositoryTests
{
    private IContentTypeRepository _contentTypeRepository;
    private SciMaterialsContext _context;

    public ContentTypeRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _contentTypeRepository = new ContentTypeRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_AsNoTracking_ItShould_contains_contentTypes()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var contentTypes = _contentTypeRepository.GetAll();

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedState, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetAll_Tracking_ItShould_contains_contentTypes()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = _contentTypeRepository.GetAll(false);

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_contentTypes()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var contentTypes = await _contentTypeRepository.GetAllAsync();

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetAllAsync_Tracking_ItShould_contains_contentTypes()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = await _contentTypeRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void AddAsync_ItShould_contains_contentType_increase_by_1()
    {
        //arrange
        var expected = (await _contentTypeRepository.GetAllAsync())!.Count + 1;
        var contentType = ContentTypeHelper.GetOne();

        //act
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        var contentTypes = await _contentTypeRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var contentTypeDb = await _contentTypeRepository.GetByIdAsync(contentType.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(contentType.Name, contentTypeDb.Name);
        Assert.Equal(contentType.FileExtension, contentTypeDb.FileExtension);
        Assert.Equal(contentType.Files.ToList()[0].Id, contentTypeDb.Files.ToList()[0].Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Add_ItShould_contains_contentType_3()
    {
        //arrange
        var expected = _contentTypeRepository.GetAll()!.Count + 1;
        var contentType = ContentTypeHelper.GetOne();

        //act
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        var contentTypes = _contentTypeRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var contentTypeDb = _contentTypeRepository.GetById(contentType.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(contentType.Name, contentTypeDb.Name);
        Assert.Equal(contentType.FileExtension, contentTypeDb.FileExtension);
        Assert.Equal(contentType.Files.ToList()[0].Id, contentTypeDb.Files.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();
        var expected = (await _contentTypeRepository.GetAllAsync())!.Count - 1;

        //act
        await _contentTypeRepository.DeleteAsync(contentType.Id);
        await _context.SaveChangesAsync();

        var contentTypes = await _contentTypeRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedContentType = await _contentTypeRepository.GetByIdAsync(contentType.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedContentType);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();
        var expected = _contentTypeRepository.GetAll()!.Count - 1;

        //act
        _contentTypeRepository.Delete(contentType.Id);
        _context.SaveChanges();

        var contentTypes = _contentTypeRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedContentType = _contentTypeRepository.GetById(contentType.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedContentType);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByIdAsync(contentType.Id, false);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByIdAsync(contentType.Id, true);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetById(contentType.Id, false);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetById(contentType.Id, true);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByIdAsync(contentType.Id);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetById(contentType.Id);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByNameAsync(contentType.Name, true);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetByName(contentType.Name, false);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByNameAsync(contentType.Name, false);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetByName(contentType.Name, true);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
        Assert.Equal(expecedState, _context.Entry(contentTypeDb).State);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        var contentTypeDb = await _contentTypeRepository.GetByNameAsync(contentType.Name);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        var contentTypeDb = _contentTypeRepository.GetByName(contentType.Name);

        //assert
        Assert.NotNull(contentTypeDb);
        Assert.Equal(contentType.Name, contentTypeDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var contentType = ContentTypeHelper.GetOne();
        await _contentTypeRepository.AddAsync(contentType);
        await _context.SaveChangesAsync();

        //act
        contentType.Name = expectedName;
        await _contentTypeRepository.UpdateAsync(contentType);
        await _context.SaveChangesAsync();

        var contentTypeDb = await _contentTypeRepository.GetByIdAsync(contentType.Id);

        //assert
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(contentType.Name, expectedName);
    }

    [Fact]
    [Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var contentType = ContentTypeHelper.GetOne();
        _contentTypeRepository.Add(contentType);
        _context.SaveChanges();

        //act
        contentType.Name = expectedName;
        _contentTypeRepository.Update(contentType);
        _context.SaveChanges();

        var contentTypeDb = _contentTypeRepository.GetById(contentType.Id);

        //assert
        Assert.Equal(contentType.Id, contentTypeDb!.Id);
        Assert.Equal(contentType.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var contentTypeDb = await _contentTypeRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(contentTypeDb);
    //}

    //[Fact]
    //[Trait("ContentTypeRepositoryTests", nameof(ContentType))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var contentTypeDb = _contentTypeRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(contentTypeDb);
    //}

    #endregion
}
