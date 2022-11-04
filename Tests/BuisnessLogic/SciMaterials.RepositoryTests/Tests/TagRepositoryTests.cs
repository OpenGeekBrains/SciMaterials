using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;
using SciMaterials.RepositoryTests.Helpers;

namespace SciMaterials.RepositoryTests.Tests;

public class TagRepositoryTests
{
    private ITagRepository _tagRepository;
    private SciMaterialsContext _context;

    public TagRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _tagRepository = new TagRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetAll_AsNoTracking_ItShould_contains_tags()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var tags = _tagRepository.GetAll();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedState, _context.Entry(tags![0]).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetAll_Tracking_ItShould_contains_tags()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var tags = _tagRepository.GetAll(false);

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _context.Entry(tags![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_tags()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var tags = await _tagRepository.GetAllAsync();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _context.Entry(tags![0]).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetAllAsync_Tracking_ItShould_contains_tags()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var tags = await _tagRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _context.Entry(tags![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void AddAsync_ItShould_contains_tag_increase_by_1()
    {
        //arrange
        var expected = (await _tagRepository.GetAllAsync())!.Count + 1;
        var tag = TagHelper.GetOne();

        //act
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        var tags = await _tagRepository.GetAllAsync();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var tagDb = await _tagRepository.GetByIdAsync(tag.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, tagDb.Name);
        Assert.Equal(tag.Files.ToList()[0].Id, tagDb.Files.ToList()[0].Id);
        Assert.Equal(tag.FileGroups.ToList()[0].Id, tagDb.FileGroups.ToList()[0].Id);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void Add_ItShould_contains_tag_3()
    {
        //arrange
        var expected = _tagRepository.GetAll()!.Count + 1;
        var tag = TagHelper.GetOne();

        //act
        _tagRepository.Add(tag);
        _context.SaveChanges();

        var tags = _tagRepository.GetAll();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var tagDb = _tagRepository.GetById(tag.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, tagDb.Name);
        Assert.Equal(tag.Files.ToList()[0].Id, tagDb.Files.ToList()[0].Id);
        Assert.Equal(tag.FileGroups.ToList()[0].Id, tagDb.FileGroups.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();
        var expected = (await _tagRepository.GetAllAsync())!.Count - 1;

        //act
        await _tagRepository.DeleteAsync(tag.Id);
        await _context.SaveChangesAsync();

        var tags = await _tagRepository.GetAllAsync();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var removedTag = await _tagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedTag);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();
        var expected = _tagRepository.GetAll()!.Count - 1;

        //act
        _tagRepository.Delete(tag.Id);
        _context.SaveChanges();

        var tags = _tagRepository.GetAll();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var removedTag = _tagRepository.GetById(tag.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedTag);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByIdAsync(tag.Id, false);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByIdAsync(tag.Id, true);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetById(tag.Id, false);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetById(tag.Id, true);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetById(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByNameAsync(tag.Name, true);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetByName(tag.Name, false);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByNameAsync(tag.Name, false);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetByName(tag.Name, true);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
        Assert.Equal(expecedState, _context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        var tagDb = await _tagRepository.GetByNameAsync(tag.Name);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        var tagDb = _tagRepository.GetByName(tag.Name);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Name, tagDb!.Name);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var tag = TagHelper.GetOne();
        await _tagRepository.AddAsync(tag);
        await _context.SaveChangesAsync();

        //act
        tag.Name = expectedName;
        await _tagRepository.UpdateAsync(tag);
        await _context.SaveChangesAsync();

        var tagDb = await _tagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, expectedName);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var tag = TagHelper.GetOne();
        _tagRepository.Add(tag);
        _context.SaveChanges();

        //act
        tag.Name = expectedName;
        _tagRepository.Update(tag);
        _context.SaveChanges();

        var tagDb = _tagRepository.GetById(tag.Id);

        //assert
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("TagRepositoryTests", nameof(Tag))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var tagDb = await _tagRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(tagDb);
    //}

    //[Fact]
    //[Trait("TagRepositoryTests", nameof(Tag))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var tagDb = _tagRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(tagDb);
    //}

    #endregion
}
