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
using SciMaterials.RepositoryLib.Repositories.UrlsRepositories;

namespace SciMaterials.RepositoryTests.Tests;

public class UrlRepositoryTests
{
    private IUrlRepository _urlGroupRepository;
    private SciMaterialsContext _context;

    public UrlRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _urlGroupRepository = new UrlRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetAll_AsNoTracking_ItShould_contains_urls()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var urls = _urlGroupRepository.GetAll();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedState, _context.Entry(urls![0]).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetAll_Tracking_ItShould_contains_urls()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var urls = _urlGroupRepository.GetAll(false);

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _context.Entry(urls![0]).State);
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
        var urls = await _urlGroupRepository.GetAllAsync();

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _context.Entry(urls![0]).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetAllAsync_Tracking_ItShould_contains_urls()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var urls = await _urlGroupRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(urls);
        Assert.Equal(expecedSstate, _context.Entry(urls![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void AddAsync_ItShould_contains_url_increase_by_1()
    {
        //arrange
        var expected = (await _urlGroupRepository.GetAllAsync())!.Count + 1;
        var url = UrlHelper.GetOne();

        //act
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        var urls = await _urlGroupRepository.GetAllAsync();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var urlDb = await _urlGroupRepository.GetByIdAsync(url.Id, include: true);

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
        //arrange
        var expected = _urlGroupRepository.GetAll()!.Count + 1;
        var url = UrlHelper.GetOne();

        //act
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        var urls = _urlGroupRepository.GetAll();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var urlDb = _urlGroupRepository.GetById(url.Id, include: true);

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
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();
        var expected = (await _urlGroupRepository.GetAllAsync())!.Count - 1;

        //act
        await _urlGroupRepository.DeleteAsync(url.Id);
        await _context.SaveChangesAsync();

        var urls = await _urlGroupRepository.GetAllAsync();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var removedUrl = await _urlGroupRepository.GetByIdAsync(url.Id);

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
        _urlGroupRepository.Add(url);
        _context.SaveChanges();
        var expected = _urlGroupRepository.GetAll()!.Count - 1;

        //act
        _urlGroupRepository.Delete(url.Id);
        _context.SaveChanges();

        var urls = _urlGroupRepository.GetAll();
        var count = 0;
        if (urls is not null)
            count = urls.Count;

        var removedUrl = _urlGroupRepository.GetById(url.Id);

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
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByIdAsync(url.Id, false);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByIdAsync(url.Id, true);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetById(url.Id, false);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetById(url.Id, true);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByIdAsync(url.Id);

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
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetById(url.Id);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Id, urlDb!.Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByNameAsync(url.Name, true);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetByName(url.Name, false);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByNameAsync(url.Name, false);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var url = UrlHelper.GetOne();
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetByName(url.Name, true);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
        Assert.Equal(expecedState, _context.Entry(urlDb).State);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var url = UrlHelper.GetOne();
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        var urlDb = await _urlGroupRepository.GetByNameAsync(url.Name);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
    }

    [Fact]
    [Trait("UrlRepositoryTests", nameof(Url))]
    public void GetByName_ItShould_entity_not_null_and_equals_name()
    {
        //arrange
        var url = UrlHelper.GetOne();
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        var urlDb = _urlGroupRepository.GetByName(url.Name);

        //assert
        Assert.NotNull(urlDb);
        Assert.Equal(url.Name, urlDb!.Name);
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
        await _urlGroupRepository.AddAsync(url);
        await _context.SaveChangesAsync();

        //act
        url.Name = expectedName;
        await _urlGroupRepository.UpdateAsync(url);
        await _context.SaveChangesAsync();

        var urlDb = await _urlGroupRepository.GetByIdAsync(url.Id);

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
        _urlGroupRepository.Add(url);
        _context.SaveChanges();

        //act
        url.Name = expectedName;
        _urlGroupRepository.Update(url);
        _context.SaveChanges();

        var urlDb = _urlGroupRepository.GetById(url.Id);

        //assert
        Assert.Equal(url.Id, urlDb!.Id);
        Assert.Equal(url.Name, expectedName);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("UrlRepositoryTests", nameof(Url))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var urlDb = await _urlGroupRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(urlDb);
    //}

    //[Fact]
    //[Trait("UrlRepositoryTests", nameof(Url))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var urlDb = _urlGroupRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(urlDb);
    //}

    #endregion
}
