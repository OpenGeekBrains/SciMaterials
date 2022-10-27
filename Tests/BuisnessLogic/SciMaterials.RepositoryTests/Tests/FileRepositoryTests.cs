using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Tests;

public class FileRepositoryTests
{
    private IFileRepository _fileRepository;
    private SciMaterialsContext _context;

    public FileRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _fileRepository = new FileRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetAll_AsNoTracking_ItShould_contains_files()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var contentTypes = _fileRepository.GetAll();

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedState, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetAll_Tracking_ItShould_contains_files()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = _fileRepository.GetAll(false);

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_files()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var contentTypes = await _fileRepository.GetAllAsync();

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetAllAsync_Tracking_ItShould_contains_files()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var contentTypes = await _fileRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(contentTypes);
        Assert.Equal(expecedSstate, _context.Entry(contentTypes![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void AddAsync_ItShould_contains_file_increase_by_1()
    {
        //arrange
        var expected = (await _fileRepository.GetAllAsync())!.Count + 1;
        var file = FileHelper.GetOne();

        //act
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        var contentTypes = await _fileRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var fileDb = await _fileRepository.GetByIdAsync(file.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(file.Name, fileDb.Name);
        Assert.Equal(file.ShortInfo, fileDb.ShortInfo);
        Assert.Equal(file.Description, fileDb.Description);
        Assert.Equal(file.CreatedAt, fileDb.CreatedAt);
        Assert.Equal(file.Url, fileDb.Url);
        Assert.Equal(file.Size, fileDb.Size);
        Assert.Equal(file.Hash, fileDb.Hash);
        Assert.Equal(file.Author.Id, fileDb.Author.Id);
        Assert.Equal(file.Categories.ToList()[0].Id, fileDb.Categories.ToList()[0].Id);
        Assert.Equal(file.Comments.ToList()[0].Id, fileDb.Comments.ToList()[0].Id);
        Assert.Equal(file.Ratings.ToList()[0].Id, fileDb.Ratings.ToList()[0].Id);
        Assert.Equal(file.Tags.ToList()[0].Id, fileDb.Tags.ToList()[0].Id);
        Assert.Equal(file.FileGroup.Id, fileDb.FileGroup.Id);
        Assert.Equal(file.ContentType.Id, fileDb.ContentType.Id);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void Add_ItShould_contains_file_3()
    {
        //arrange
        var expected = _fileRepository.GetAll()!.Count + 1;
        var file = FileHelper.GetOne();

        //act
        _fileRepository.Add(file);
        _context.SaveChanges();

        var contentTypes = _fileRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var fileDb = _fileRepository.GetById(file.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(file.Name, fileDb.Name);
        Assert.Equal(file.ShortInfo, fileDb.ShortInfo);
        Assert.Equal(file.Description, fileDb.Description);
        Assert.Equal(file.CreatedAt, fileDb.CreatedAt);
        Assert.Equal(file.Url, fileDb.Url);
        Assert.Equal(file.Size, fileDb.Size);
        Assert.Equal(file.Hash, fileDb.Hash);
        Assert.Equal(file.Author.Id, fileDb.Author.Id);
        Assert.Equal(file.Categories.ToList()[0].Id, fileDb.Categories.ToList()[0].Id);
        Assert.Equal(file.Comments.ToList()[0].Id, fileDb.Comments.ToList()[0].Id);
        Assert.Equal(file.Ratings.ToList()[0].Id, fileDb.Ratings.ToList()[0].Id);
        Assert.Equal(file.Tags.ToList()[0].Id, fileDb.Tags.ToList()[0].Id);
        Assert.Equal(file.FileGroup.Id, fileDb.FileGroup.Id);
        Assert.Equal(file.ContentType.Id, fileDb.ContentType.Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();
        var expected = (await _fileRepository.GetAllAsync())!.Count - 1;

        //act
        await _fileRepository.DeleteAsync(file.Id);
        await _context.SaveChangesAsync();

        var contentTypes = await _fileRepository.GetAllAsync();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedFile = await _fileRepository.GetByIdAsync(file.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedFile);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();
        var expected = _fileRepository.GetAll()!.Count - 1;

        //act
        _fileRepository.Delete(file.Id);
        _context.SaveChanges();

        var contentTypes = _fileRepository.GetAll();
        var count = 0;
        if (contentTypes is not null)
            count = contentTypes.Count;

        var removedFile = _fileRepository.GetById(file.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedFile);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByIdAsync(file.Id, false);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByIdAsync(file.Id, true);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetById(file.Id, false);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetById(file.Id, true);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByIdAsync(file.Id);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetById(file.Id);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Id, fileDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByNameAsync(file.Name, true);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetByName(file.Name, false);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByNameAsync(file.Name, false);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetByName(file.Name, true);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
        Assert.Equal(expecedState, _context.Entry(fileDb).State);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        var fileDb = await _fileRepository.GetByNameAsync(file.Name);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        var fileDb = _fileRepository.GetByName(file.Name);

        //assert
        Assert.NotNull(fileDb);
        Assert.Equal(file.Name, fileDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var file = FileHelper.GetOne();
        await _fileRepository.AddAsync(file);
        await _context.SaveChangesAsync();

        //act
        file.Name = expectedName;
        await _fileRepository.UpdateAsync(file);
        await _context.SaveChangesAsync();

        var fileDb = await _fileRepository.GetByIdAsync(file.Id);

        //assert
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(file.Name, expectedName);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var file = FileHelper.GetOne();
        _fileRepository.Add(file);
        _context.SaveChanges();

        //act
        file.Name = expectedName;
        _fileRepository.Update(file);
        _context.SaveChanges();

        var fileDb = _fileRepository.GetById(file.Id);

        //assert
        Assert.Equal(file.Id, fileDb!.Id);
        Assert.Equal(file.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByHashAsync_ItShould_null()
    {
        //arrange

        //act
        var fileDb = await _fileRepository.GetByHashAsync(String.Empty);

        //assert
        Assert.Null(fileDb);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var fileDb = _fileRepository.GetByHash(String.Empty);

        //assert
        Assert.Null(fileDb);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public async void GetByHashAsync_ItShould_NotNull()
    {
        //arrange
        var expected = "Hash";

        //act
        var fileDb = await _fileRepository.GetByHashAsync(expected);

        //assert
        Assert.NotNull(fileDb);
    }

    [Fact]
    [Trait("FileRepositoryTests", nameof(File))]
    public void GetByHash_ItShould_NotNull()
    {
        //arrange
        var expected = "Hash";

        //act
        var fileDb = _fileRepository.GetByHash(expected);

        //assert
        Assert.NotNull(fileDb);
    }
    #endregion
}
