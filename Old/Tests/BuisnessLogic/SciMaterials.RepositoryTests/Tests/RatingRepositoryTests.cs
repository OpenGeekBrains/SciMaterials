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
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Ratings;
using SciMaterials.DAL.Resources.Repositories.Ratings;
using SciMaterials.DAL.Resources.UnitOfWork;
using Moq;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.RepositoryTests.Tests;

public class RatingRepositoryTests
{
    private RatingRepository _RatingRepository;
    private SciMaterialsContext _Context;

    public RatingRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<RatingRepository>>();
        _RatingRepository = new RatingRepository(_Context, logger.Object);
    }


    #region GetAll

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetAll_AsNoTracking_ItShould_contains_ratings()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;

        //act
        var ratings = _RatingRepository.GetAll();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedState, _Context.Entry(ratings![0]).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetAll_Tracking_ItShould_contains_ratings()
    {
        _RatingRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var ratings = _RatingRepository.GetAll();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _Context.Entry(ratings![0]).State);
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
        var ratings = await _RatingRepository.GetAllAsync();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _Context.Entry(ratings![0]).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetAllAsync_Tracking_ItShould_contains_ratings()
    {
        _RatingRepository.NoTracking = false;
        //arrange
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var ratings = await _RatingRepository.GetAllAsync();

        //assert
        Assert.NotNull(ratings);
        Assert.Equal(expecedSstate, _Context.Entry(ratings![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void AddAsync_ItShould_contains_rating_increase_by_1()
    {
        _RatingRepository.Include = true;
        //arrange
        var expected = (await _RatingRepository.GetAllAsync())!.Count + 1;
        var rating = RatingHelper.GetOne();

        //act
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();

        var ratings = await _RatingRepository.GetAllAsync();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var ratingDb = await _RatingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, ratingDb.RatingValue);
        Assert.Equal(rating.User.Id, ratingDb.User.Id);
        Assert.Equal(rating.Resource.Id, ratingDb.Resource.Id);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void Add_ItShould_contains_rating_3()
    {
        _RatingRepository.Include = true;
        //arrange
        var expected = _RatingRepository.GetAll()!.Count + 1;
        var rating = RatingHelper.GetOne();

        //act
        _RatingRepository.Add(rating);
        _Context.SaveChanges();

        var ratings = _RatingRepository.GetAll();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var ratingDb = _RatingRepository.GetById(rating.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, ratingDb.RatingValue);
        Assert.Equal(rating.User.Id, ratingDb.User.Id);
        Assert.Equal(rating.Resource.Id, ratingDb.Resource.Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();
        var expected = (await _RatingRepository.GetAllAsync())!.Count - 1;

        //act
        await _RatingRepository.DeleteAsync(rating.Id);
        await _Context.SaveChangesAsync();

        var ratings = await _RatingRepository.GetAllAsync();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var removedRating = await _RatingRepository.GetByIdAsync(rating.Id);

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
        _RatingRepository.Add(rating);
        _Context.SaveChanges();
        var expected = _RatingRepository.GetAll()!.Count - 1;

        //act
        _RatingRepository.Delete(rating.Id);
        _Context.SaveChanges();

        var ratings = _RatingRepository.GetAll();
        var count = 0;
        if (ratings is not null)
            count = ratings.Count;

        var removedRating = _RatingRepository.GetById(rating.Id);

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
        _RatingRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var rating = RatingHelper.GetOne();
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();

        //act
        var ratingDb = await _RatingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        _RatingRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var rating = RatingHelper.GetOne();
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();

        //act
        var ratingDb = await _RatingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _RatingRepository.NoTracking = false;
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var rating = RatingHelper.GetOne();
        _RatingRepository.Add(rating);
        _Context.SaveChanges();

        //act
        var ratingDb = _RatingRepository.GetById(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        _RatingRepository.NoTracking = true;
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var rating = RatingHelper.GetOne();
        _RatingRepository.Add(rating);
        _Context.SaveChanges();

        //act
        var ratingDb = _RatingRepository.GetById(rating.Id);

        //assert
        Assert.NotNull(ratingDb);
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(expecedState, _Context.Entry(ratingDb).State);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var rating = RatingHelper.GetOne();
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();

        //act
        var ratingDb = await _RatingRepository.GetByIdAsync(rating.Id);

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
        _RatingRepository.Add(rating);
        _Context.SaveChanges();

        //act
        var ratingDb = _RatingRepository.GetById(rating.Id);

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
        _RatingRepository.NoTracking = false;
        //arrange
        var expectedName = 4;

        var rating = RatingHelper.GetOne();
        await _RatingRepository.AddAsync(rating);
        await _Context.SaveChangesAsync();

        //act
        rating.RatingValue = expectedName;
        await _RatingRepository.UpdateAsync(rating);
        await _Context.SaveChangesAsync();

        var ratingDb = await _RatingRepository.GetByIdAsync(rating.Id);

        //assert
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, expectedName);
    }

    [Fact]
    [Trait("RatingRepositoryTests", nameof(Rating))]
    public void Update_ItShould_properties_updated()
    {
        _RatingRepository.NoTracking = false;
        //arrange
        var expectedName = 4;

        var rating = RatingHelper.GetOne();
        _RatingRepository.Add(rating);
        _Context.SaveChanges();

        //act
        rating.RatingValue = expectedName;
        _RatingRepository.Update(rating);
        _Context.SaveChanges();

        var ratingDb = _RatingRepository.GetById(rating.Id);

        //assert
        Assert.Equal(rating.Id, ratingDb!.Id);
        Assert.Equal(rating.RatingValue, expectedName);
    }

    #endregion
}