using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;
using SciMaterials.RepositoryTests.Helpers;
using Moq;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.RepositoryTests.Tests;

public class UrlRepositoryTests
{
    private UrlRepository _UrlGroupRepository;
    private SciMaterialsContext _Context;

    public UrlRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<UrlRepository>>();
        _UrlGroupRepository = new UrlRepository(_Context, logger.Object);
    }


    #region GetAll

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetAll_AsNoTracking_ItShould_contains_urls()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var urls = _UrlGroupRepository.GetAll();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedState, _Context.Entry(urls![0]).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetAll_Tracking_ItShould_contains_urls()
    {
        _UrlGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var urls = _UrlGroupRepository.GetAll();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _Context.Entry(urls![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_urls()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var urls = await _UrlGroupRepository.GetAllAsync();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _Context.Entry(urls![0]).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetAllAsync_Tracking_ItShould_contains_urls()
    {
        _UrlGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var urls = await _UrlGroupRepository.GetAllAsync();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _Context.Entry(urls![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void AddAsync_ItShould_contains_url_increase_by_1()
    {
        _UrlGroupRepository.Include = true;
        //arrange
        var expected = (await _UrlGroupRepository.GetAllAsync())!.Count + 1;
        var url = UrlHelper.GetOne();

        //act
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();

        var urls = await _UrlGroupRepository.GetAllAsync();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var urlDb = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(url.Name, urlDb.Name);
        Assert.Equal(url.Author.Id, urlDb.Author.Id);
        Assert.Equal(url.Description, urlDb.Description);
        Assert.Equal(url.ShortInfo, urlDb.ShortInfo);
        Assert.Equal(url.CreatedAt, urlDb.CreatedAt);
        Assert.Equal(url.Link, urlDb.Link);
        Assert.Equal(url.Categories.ToList()[0].Id, urlDb.Categories.ToList()[0].Id);
        Assert.Equal(url.Comments.ToList()[0].Id, urlDb.Comments.ToList()[0].Id);
        Assert.Equal(url.Ratings.ToList()[0].Id, urlDb.Ratings.ToList()[0].Id);
        Assert.Equal(url.Tags.ToList()[0].Id, urlDb.Tags.ToList()[0].Id);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void Add_ItShould_contains_url_3()
    {
        _UrlGroupRepository.Include = true;
        //arrange
        var expected = _UrlGroupRepository.GetAll()!.Count + 1;
        var url = UrlHelper.GetOne();

        //act
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();

        var urls = _UrlGroupRepository.GetAll();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var urlDb = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(url.Name, urlDb.Name);
        Assert.Equal(url.Author.Id, urlDb.Author.Id);
        Assert.Equal(url.Description, urlDb.Description);
        Assert.Equal(url.ShortInfo, urlDb.ShortInfo);
        Assert.Equal(url.CreatedAt, urlDb.CreatedAt);
        Assert.Equal(url.Link, urlDb.Link);
        Assert.Equal(url.Categories.ToList()[0].Id, urlDb.Categories.ToList()[0].Id);
        Assert.Equal(url.Comments.ToList()[0].Id, urlDb.Comments.ToList()[0].Id);
        Assert.Equal(url.Ratings.ToList()[0].Id, urlDb.Ratings.ToList()[0].Id);
        Assert.Equal(url.Tags.ToList()[0].Id, urlDb.Tags.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var url = UrlHelper.GetOne();
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();
        var expected = (await _UrlGroupRepository.GetAllAsync())!.Count - 1;

        //act
        await _UrlGroupRepository.DeleteAsync(url.Id);
        await _Context.SaveChangesAsync();

        var urls = await _UrlGroupRepository.GetAllAsync();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var removedUrl = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedUrl);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var url = UrlHelper.GetOne();
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();
        var expected = _UrlGroupRepository.GetAll()!.Count - 1;

        //act
        _UrlGroupRepository.Delete(url.Id);
        _Context.SaveChanges();

        var urls = _UrlGroupRepository.GetAll();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var removedUrl = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedUrl);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        _UrlGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();

        //act
        var urlDb = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        _UrlGroupRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();

        //act
        var urlDb = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _UrlGroupRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();

        //act
        var urlDb = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        _UrlGroupRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();

        //act
        var urlDb = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var url = UrlHelper.GetOne();
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();

        //act
        var urlDb = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var url = UrlHelper.GetOne();
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();

        //act
        var urlDb = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var url = UrlHelper.GetOne();
        await _UrlGroupRepository.AddAsync(url);
        await _Context.SaveChangesAsync();

        //act
        url.Name = expectedName;
        await _UrlGroupRepository.UpdateAsync(url);
        await _Context.SaveChangesAsync();

        var urlDb = await _UrlGroupRepository.GetByIdAsync(url.Id);

        //assert
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(url.Name, expectedName);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = "new content type name";

        var url = UrlHelper.GetOne();
        _UrlGroupRepository.Add(url);
        _Context.SaveChanges();

        //act
        url.Name = expectedName;
        _UrlGroupRepository.Update(url);
        _Context.SaveChanges();

        var urlDb = _UrlGroupRepository.GetById(url.Id);

        //assert
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(url.Name, expectedName);
    }

    #endregion
}