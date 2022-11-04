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
using SciMaterials.RepositoryLib.Repositories.RatingRepositories;

namespace SciMaterials.RepositoryTests.Tests;

public class RatingRepositoryTests
{
    private IRatingRepository _ratingRepository;
    private SciMaterialsContext _context;

    public RatingRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _ratingRepository = new RatingRepository(_context, logger);
    }


    #region GetAll

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetAll_AsNoTracking_ItShould_contains_ratings()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var ratings = _ratingRepository.GetAll();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedState, _context.Entry(ratings![0]).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetAll_Tracking_ItShould_contains_ratings()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var ratings = _ratingRepository.GetAll(false);

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _context.Entry(ratings![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_ratings()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var ratings = await _ratingRepository.GetAllAsync();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _context.Entry(ratings![0]).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetAllAsync_Tracking_ItShould_contains_ratings()
    {
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var ratings = await _ratingRepository.GetAllAsync(false);

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _context.Entry(ratings![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void AddAsync_ItShould_contains_rating_increase_by_1()
    {
        //arrange
        var expected = (await _ratingRepository.GetAllAsync())!.Count + 1;
        var rating = RatingHelper.GetOne();

        //act
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();

        var ratings = await _ratingRepository.GetAllAsync();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var ratingDb = await _ratingRepository.GetByIdAsync(rating.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, ratingDb.RatingValue);
        Assert.Equal(rating.User.Id, ratingDb.User.Id);
        Assert.Equal(rating.File.Id, ratingDb.File.Id);
        Assert.Equal(rating.FileGroup.Id, ratingDb.FileGroup.Id);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void Add_ItShould_contains_rating_3()
    {
        //arrange
        var expected = _ratingRepository.GetAll()!.Count + 1;
        var rating = RatingHelper.GetOne();

        //act
        _ratingRepository.Add(rating);
        _context.SaveChanges();

        var ratings = _ratingRepository.GetAll();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var ratingDb = _ratingRepository.GetById(rating.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, ratingDb.RatingValue);
        Assert.Equal(rating.User.Id, ratingDb.User.Id);
        Assert.Equal(rating.File.Id, ratingDb.File.Id);
        Assert.Equal(rating.FileGroup.Id, ratingDb.FileGroup.Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();
        var expected = (await _ratingRepository.GetAllAsync())!.Count - 1;

        //act
        await _ratingRepository.DeleteAsync(rating.Id);
        await _context.SaveChangesAsync();

        var ratings = await _ratingRepository.GetAllAsync();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var removedRating = await _ratingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedRating);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        _ratingRepository.Add(rating);
        _context.SaveChanges();
        var expected = _ratingRepository.GetAll()!.Count - 1;

        //act
        _ratingRepository.Delete(rating.Id);
        _context.SaveChanges();

        var ratings = _ratingRepository.GetAll();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var removedRating = _ratingRepository.GetById(rating.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedRating);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var rating = RatingHelper.GetOne();
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();

        //act
        var ratingDb = await _ratingRepository.GetByIdAsync(rating.Id, false);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var rating = RatingHelper.GetOne();
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();

        //act
        var ratingDb = await _ratingRepository.GetByIdAsync(rating.Id, true);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var rating = RatingHelper.GetOne();
        _ratingRepository.Add(rating);
        _context.SaveChanges();

        //act
        var ratingDb = _ratingRepository.GetById(rating.Id, false);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var rating = RatingHelper.GetOne();
        _ratingRepository.Add(rating);
        _context.SaveChanges();

        //act
        var ratingDb = _ratingRepository.GetById(rating.Id, true);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();

        //act
        var ratingDb = await _ratingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        _ratingRepository.Add(rating);
        _context.SaveChanges();

        //act
        var ratingDb = _ratingRepository.GetById(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName = 4;

        var rating = RatingHelper.GetOne();
        await _ratingRepository.AddAsync(rating);
        await _context.SaveChangesAsync();

        //act
        rating.RatingValue = expectedName;
        await _ratingRepository.UpdateAsync(rating);
        await _context.SaveChangesAsync();

        var ratingDb = await _ratingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, expectedName);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName = 4;

        var rating = RatingHelper.GetOne();
        _ratingRepository.Add(rating);
        _context.SaveChanges();

        //act
        rating.RatingValue = expectedName;
        _ratingRepository.Update(rating);
        _context.SaveChanges();

        var ratingDb = _ratingRepository.GetById(rating.Id);

        //assert
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, expectedName);
    }

    #endregion
}
