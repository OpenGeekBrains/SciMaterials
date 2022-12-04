using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;
using SciMaterials.DAL.Resources.Contracts.Repositories.Users;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class FileGroupRepositoryTests
{
    private readonly FileGroupRepository _FileGroupRepository;
    private readonly SciMaterialsContext _Context;

    public FileGroupRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<FileGroupRepository>>();
        _FileGroupRepository = new FileGroupRepository(_Context, logger.Object);
    }


    #region GetAll

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetAll_AsNoTracking_ItShould_contains_fileGroups()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var fileGroups = _FileGroupRepository.GetAll();

        //assert
        Assert.NotNull(fileGroups);
        Assert.Equal(expecedState, _Context.Entry(fileGroups![0]).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetAll_Tracking_ItShould_contains_fileGroups()
    {
        _FileGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var fileGroups = _FileGroupRepository.GetAll();

        //assert
        Assert.NotNull(fileGroups);
        Assert.Equal(expecedSstate, _Context.Entry(fileGroups![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_fileGroups()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var fileGroups = await _FileGroupRepository.GetAllAsync();

        //assert
        Assert.NotNull(fileGroups);
        Assert.Equal(expecedSstate, _Context.Entry(fileGroups![0]).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetAllAsync_Tracking_ItShould_contains_fileGroups()
    {
        _FileGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var fileGroups = await _FileGroupRepository.GetAllAsync();

        //assert
        Assert.NotNull(fileGroups);
        Assert.Equal(expecedSstate, _Context.Entry(fileGroups![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void AddAsync_ItShould_contains_fileGroup_increase_by_1()
    {
        _FileGroupRepository.Include = true;
        //arrange
        var expected = (await _FileGroupRepository.GetAllAsync())!.Count + 1;
        var fileGroup = FileGroupHelper.GetOne();

        //act
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();

        var fileGroups = await _FileGroupRepository.GetAllAsync();
        var count = 0;
        if (fileGroups is not null)
            count = fileGroups.Count;

        var fileGroupDb = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

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
        _FileGroupRepository.Include = true;
        //arrange
        var expected = _FileGroupRepository.GetAll()!.Count + 1;
        var fileGroup = FileGroupHelper.GetOne();

        //act
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();

        var fileGroups = _FileGroupRepository.GetAll();
        var count = 0;
        if (fileGroups is not null)
            count = fileGroups.Count;

        var fileGroupDb = _FileGroupRepository.GetById(fileGroup.Id);

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
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();
        var expected = (await _FileGroupRepository.GetAllAsync())!.Count - 1;

        //act
        await _FileGroupRepository.DeleteAsync(fileGroup.Id);
        await _Context.SaveChangesAsync();

        var fileGroups = await _FileGroupRepository.GetAllAsync();
        var count = 0;
        if (fileGroups is not null)
            count = fileGroups.Count;

        var removedFileGroup = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

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
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();
        var expected = _FileGroupRepository.GetAll()!.Count - 1;

        //act
        _FileGroupRepository.Delete(fileGroup.Id);
        _Context.SaveChanges();

        var fileGroups = _FileGroupRepository.GetAll();
        var count = 0;
        if (fileGroups is not null)
            count = fileGroups.Count;

        var removedFileGroup = _FileGroupRepository.GetById(fileGroup.Id);

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
        _FileGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();

        //act
        var fileGroupDb = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        _FileGroupRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();

        //act
        var fileGroupDb = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _FileGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var fileGroup = FileGroupHelper.GetOne();
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();

        //act
        var fileGroupDb = _FileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        _FileGroupRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var fileGroup = FileGroupHelper.GetOne();
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();

        //act
        var fileGroupDb = _FileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(fileGroupDb).State);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var fileGroup = FileGroupHelper.GetOne();
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();

        //act
        var fileGroupDb = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

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
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();

        //act
        var fileGroupDb = _FileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.NotNull(fileGroupDb);
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _FileGroupRepository.NoTracking = false;

        //arrange
        var expectedName = "new content type name";

        var fileGroup = FileGroupHelper.GetOne();
        await _FileGroupRepository.AddAsync(fileGroup);
        await _Context.SaveChangesAsync();

        //act
        fileGroup.Name = expectedName;
        await _FileGroupRepository.UpdateAsync(fileGroup);
        await _Context.SaveChangesAsync();

        var fileGroupDb = await _FileGroupRepository.GetByIdAsync(fileGroup.Id);

        //assert
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, expectedName);
    }

    [Fact]
    [Trait("FileGroupRepositoryTests", nameof(FileGroup))]
    public void Update_ItShould_properties_updated()
    {
        _FileGroupRepository.NoTracking = false;

        //arrange
        var expectedName = "new content type name";

        var fileGroup = FileGroupHelper.GetOne();
        _FileGroupRepository.Add(fileGroup);
        _Context.SaveChanges();

        //act
        fileGroup.Name = expectedName;
        _FileGroupRepository.Update(fileGroup);
        _Context.SaveChanges();

        var fileGroupDb = _FileGroupRepository.GetById(fileGroup.Id);

        //assert
        Assert.Equal(fileGroup.Id, fileGroupDb!.Id);
        Assert.Equal(fileGroup.Name, expectedName);
    }

    #endregion
}