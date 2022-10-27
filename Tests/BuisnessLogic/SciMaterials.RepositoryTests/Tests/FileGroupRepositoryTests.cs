using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class FileGroupRepositoryTests
{
    private IFileGroupRepository _fileGroupRepository;
    private SciMaterialsContext _context;

    public FileGroupRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _fileGroupRepository = new FileGroupRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetAll_AsNoTracking_ItShould_contains_fileGroup_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedState = EntityState.Detached;

        //act
        var contentTypes = _fileGroupRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedState, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetAll_Tracking_ItShould_contains_fileGroup_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = _fileGroupRepository.GetAll(false);
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_fileGroup_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var contentTypes = await _fileGroupRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetAllAsync_Tracking_ItShould_contains_fileGroup_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = await _fileGroupRepository.GetAllAsync(false);
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void AddAsync_ItShould_contains_fileGroup_increase_by_1()
    {
        //arrange
        var expected = (await _fileGroupRepository.GetAllAsync())!.Count + 1;
        var fileGroup = FileGroupHelper.GetOne();

        //act
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        var contentTypes = await _fileGroupRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var fileGroupDb = await _fileGroupRepository.GetByIdAsync(fileGroup.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, fileGroupDb.Name);
        Assert.Equal(fileGroup.Author.Id, fileGroupDb.Author.Id);
        Assert.Equal(fileGroup.Description, fileGroupDb.Description);
        Assert.Equal(fileGroup.ShortInfo, fileGroupDb.ShortInfo);
        Assert.Equal(fileGroup.CreatedAt, fileGroupDb.CreatedAt);
        Assert.Equal(fileGroup.Files.ToList()[0].Id, fileGroupDb.Files.ToList()[0].Id);
        Assert.Equal(fileGroup.Categories.ToList()[0].Id, fileGroupDb.Categories.ToList()[0].Id);
        Assert.Equal(fileGroup.Comments.ToList()[0].Id, fileGroupDb.Comments.ToList()[0].Id);
        Assert.Equal(fileGroup.Ratings.ToList()[0].Id, fileGroupDb.Ratings.ToList()[0].Id);
        Assert.Equal(fileGroup.Tags.ToList()[0].Id, fileGroupDb.Tags.ToList()[0].Id);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void Add_ItShould_contains_fileGroup_3()
    {
        //arrange
        var expected = _fileGroupRepository.GetAll()!.Count + 1;
        var fileGroup = FileGroupHelper.GetOne();

        //act
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        var contentTypes = _fileGroupRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var fileGroupDb = _fileGroupRepository.GetById(fileGroup.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, fileGroupDb.Name);
        Assert.Equal(fileGroup.Author.Id, fileGroupDb.Author.Id);
        Assert.Equal(fileGroup.Description, fileGroupDb.Description);
        Assert.Equal(fileGroup.ShortInfo, fileGroupDb.ShortInfo);
        Assert.Equal(fileGroup.CreatedAt, fileGroupDb.CreatedAt);
        Assert.Equal(fileGroup.Files.ToList()[0].Id, fileGroupDb.Files.ToList()[0].Id);
        Assert.Equal(fileGroup.Categories.ToList()[0].Id, fileGroupDb.Categories.ToList()[0].Id);
        Assert.Equal(fileGroup.Comments.ToList()[0].Id, fileGroupDb.Comments.ToList()[0].Id);
        Assert.Equal(fileGroup.Ratings.ToList()[0].Id, fileGroupDb.Ratings.ToList()[0].Id);
        Assert.Equal(fileGroup.Tags.ToList()[0].Id, fileGroupDb.Tags.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();
        var expected = (await _fileGroupRepository.GetAllAsync())!.Count - 1;

        //act
        await _fileGroupRepository.DeleteAsync(fileGroup.Id);
        await _context.SaveChangesAsync();

        var contentTypes = await _fileGroupRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedFileGroup = await _fileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedFileGroup);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();
        var expected = _fileGroupRepository.GetAll()!.Count - 1;

        //act
        _fileGroupRepository.Delete(fileGroup.Id);
        _context.SaveChanges();

        var contentTypes = _fileGroupRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedFileGroup = _fileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedFileGroup);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByIdAsync(fileGroup.Id, false);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByIdAsync(fileGroup.Id, true);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetById(fileGroup.Id, false);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetById(fileGroup.Id, true);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByNameAsync(fileGroup.Name, true);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetByName(fileGroup.Name, false);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByNameAsync(fileGroup.Name, false);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetByName(fileGroup.Name, true);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        var fileGroupDb = await _fileGroupRepository.GetByNameAsync(fileGroup.Name);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        var fileGroupDb = _fileGroupRepository.GetByName(fileGroup.Name);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Name, fileGroupDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var fileGroup = FileGroupHelper.GetOne();
        await _fileGroupRepository.AddAsync(fileGroup);
        await _context.SaveChangesAsync();

        //act
        fileGroup.Name = expectedName;
        await _fileGroupRepository.UpdateAsync(fileGroup);
        await _context.SaveChangesAsync();

        var fileGroupDb = await _fileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, expectedName);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var fileGroup = FileGroupHelper.GetOne();
        _fileGroupRepository.Add(fileGroup);
        _context.SaveChanges();

        //act
        fileGroup.Name = expectedName;
        _fileGroupRepository.Update(fileGroup);
        _context.SaveChanges();

        var fileGroupDb = _fileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var fileGroupDb = await _fileGroupRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(fileGroupDb);
    //}

    //[Fact]
    //[Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var fileGroupDb = _fileGroupRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(fileGroupDb);
    //}

    #endregion
}
