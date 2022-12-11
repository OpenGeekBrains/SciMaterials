using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.UnitOfWork;
using Moq;
using SciMaterials.DAL.Resources.Repositories.Ratings;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Extensions;
using SciMaterials.DAL.Resources.Contracts.Repositories.Ratings;

namespace SciMaterials.RepositoryTests.Tests;

public class TagRepositoryTests
{
    private TagRepository _TagRepository;
    private SciMaterialsContext _Context;

    public TagRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<TagRepository>>();
        _TagRepository = new TagRepository(_Context, logger.Object);
    }


    #region GetAll

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetAll_AsNoTracking_ItShould_contains_tags()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var tags = _TagRepository.GetAll();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedState, _Context.Entry(tags![0]).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetAll_Tracking_ItShould_contains_tags()
    {
        _TagRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var tags = _TagRepository.GetAll();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _Context.Entry(tags![0]).State);
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
        var tags = await _TagRepository.GetAllAsync();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _Context.Entry(tags![0]).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetAllAsync_Tracking_ItShould_contains_tags()
    {
        _TagRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var tags = await _TagRepository.GetAllAsync();

        //assert
        Assert.NotNull(tags);
        Assert.Equal(expecedSstate, _Context.Entry(tags![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void AddAsync_ItShould_contains_tag_increase_by_1()
    {
        _TagRepository.Include = true;
        //arrange
        var expected = (await _TagRepository.GetAllAsync())!.Count + 1;
        var tag = TagHelper.GetOne();

        //act
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();

        var tags = await _TagRepository.GetAllAsync();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var tagDb = await _TagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, tagDb.Name);
        Assert.Equal(tag.Resources.ToList()[0].Id, tagDb.Resources.ToList()[0].Id);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void Add_ItShould_contains_tag_3()
    {
        _TagRepository.Include = true;
        //arrange
        var expected = _TagRepository.GetAll()!.Count + 1;
        var tag = TagHelper.GetOne();

        //act
        _TagRepository.Add(tag);
        _Context.SaveChanges();

        var tags = _TagRepository.GetAll();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var tagDb = _TagRepository.GetById(tag.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, tagDb.Name);
        Assert.Equal(tag.Resources.ToList()[0].Id, tagDb.Resources.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var tag = TagHelper.GetOne();
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();
        var expected = (await _TagRepository.GetAllAsync())!.Count - 1;

        //act
        await _TagRepository.DeleteAsync(tag.Id);
        await _Context.SaveChangesAsync();

        var tags = await _TagRepository.GetAllAsync();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var removedTag = await _TagRepository.GetByIdAsync(tag.Id);

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
        _TagRepository.Add(tag);
        _Context.SaveChanges();
        var expected = _TagRepository.GetAll()!.Count - 1;

        //act
        _TagRepository.Delete(tag.Id);
        _Context.SaveChanges();

        var tags = _TagRepository.GetAll();
        var count = 0;
        if (tags is not null)
            count = tags.Count;

        var removedTag = _TagRepository.GetById(tag.Id);

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
        _TagRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();

        //act
        var tagDb = await _TagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        _TagRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();

        //act
        var tagDb = await _TagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _TagRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var tag = TagHelper.GetOne();
        _TagRepository.Add(tag);
        _Context.SaveChanges();

        //act
        var tagDb = _TagRepository.GetById(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        _TagRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var tag = TagHelper.GetOne();
        _TagRepository.Add(tag);
        _Context.SaveChanges();

        //act
        var tagDb = _TagRepository.GetById(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(tagDb).State);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var tag = TagHelper.GetOne();
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();

        //act
        var tagDb = await _TagRepository.GetByIdAsync(tag.Id);

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
        _TagRepository.Add(tag);
        _Context.SaveChanges();

        //act
        var tagDb = _TagRepository.GetById(tag.Id);

        //assert
        Assert.NotNull(tagDb);
        Assert.Equal(tag.Id, tagDb!.Id);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _TagRepository.NoTracking = false;
        //arrange
        var expectedName = "new content type name";

        var tag = TagHelper.GetOne();
        await _TagRepository.AddAsync(tag);
        await _Context.SaveChangesAsync();

        //act
        tag.Name = expectedName;
        await _TagRepository.UpdateAsync(tag);
        await _Context.SaveChangesAsync();

        var tagDb = await _TagRepository.GetByIdAsync(tag.Id);

        //assert
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, expectedName);
    }

    [Fact]
    [Trait("TagRepositoryTests", nameof(Tag))]
    public void Update_ItShould_properties_updated()
    {
        _TagRepository.NoTracking = false;
        //arrange
        var expectedName = "new content type name";

        var tag = TagHelper.GetOne();
        _TagRepository.Add(tag);
        _Context.SaveChanges();

        //act
        tag.Name = expectedName;
        _TagRepository.Update(tag);
        _Context.SaveChanges();

        var tagDb = _TagRepository.GetById(tag.Id);

        //assert
        Assert.Equal(tag.Id, tagDb!.Id);
        Assert.Equal(tag.Name, expectedName);
    }

    #endregion
}