using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Models.Base;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.UsersRepositories;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Tests;

public class AuthorRepositoryTests
{
    private IAuthorRepository _authorRepository;
    private SciMaterialsContext _context;

    public AuthorRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _authorRepository = new AuthorRepository(_context, logger);
    }

    #region GetAll

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var authors = _authorRepository.GetAll();
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_Tracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var authors = _authorRepository.GetAll(false);
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var authors = await _authorRepository.GetAllAsync();
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetAllAsync_Tracking_ItShould_contains_category_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var authors = await _authorRepository.GetAllAsync(false);
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(authors![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void AddAsync_ItShould_contains_category_increase_by_1()
    {
        //arrange
        var expected = (await _authorRepository.GetAllAsync())!.Count + 1;
        var author   = AuthorHelper.GetOne();

        //act
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        var authors = await _authorRepository.GetAllAsync();
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        var authorDb = await _authorRepository.GetByIdAsync(author.Id, true, true);

        ICollection<int> coll = new List<int> { 1, 2, 3 };

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(author.Name, authorDb.Name);
        Assert.Equal(author.Surname, authorDb.Surname);
        Assert.Equal(author.Email, authorDb.Email);
        Assert.Equal(author.Phone, authorDb.Phone);
        Assert.Equal(author.User!.Id, authorDb.User!.Id);
        Assert.Equal(author.Resources.ToList()[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(author.Comments.ToList()[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(author.Ratings.ToList()[0].Id, authorDb.Ratings.ToList()[0].Id);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Add_ItShould_contains_category_3()
    {
        //arrange
        var expected = _authorRepository.GetAll()!.Count + 1;
        var author = AuthorHelper.GetOne();

        //act
        _authorRepository.Add(author);
        _context.SaveChanges();

        var authors = _authorRepository.GetAll();
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        var authorDb = _authorRepository.GetById(author.Id, true, true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(author.Name, authorDb.Name);
        Assert.Equal(author.Surname, authorDb.Surname);
        Assert.Equal(author.Email, authorDb.Email);
        Assert.Equal(author.Phone, authorDb.Phone);
        Assert.Equal(author.User!.Id, authorDb.User!.Id);
        Assert.Equal(author.Resources.ToList()[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(author.Comments.ToList()[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(author.Ratings.ToList()[0].Id, authorDb.Ratings.ToList()[0].Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();
        var expected = (await _authorRepository.GetAllAsync())!.Count - 1;

        //act
        await _authorRepository.DeleteAsync(author.Id);
        await _context.SaveChangesAsync();

        var authors = await _authorRepository.GetAllAsync();
        var count   = 0;
        if (authors is not null)
            count = authors.Count;

        var removedAuthor = await _authorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedAuthor);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();
        var expected = _authorRepository.GetAll()!.Count - 1;

        //act
        _authorRepository.Delete(author.Id);
        _context.SaveChanges();

        var authors = _authorRepository.GetAll();
        var count = 0;
        if (authors is not null)
            count = authors.Count;

        var removedAuthor = _authorRepository.GetById(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedAuthor);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByIdAsync(author.Id, false);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByIdAsync(author.Id, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetById(author.Id, false);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetById(author.Id, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByIdAsync(author.Id, true, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(author.Name, authorDb.Name);
        Assert.Equal(author.Surname, authorDb.Surname);
        Assert.Equal(author.Email, authorDb.Email);
        Assert.Equal(author.Phone, authorDb.Phone);
        Assert.Equal(author.User!.Id, authorDb.User!.Id);
        Assert.Equal(author.Resources.ToList()[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(author.Comments.ToList()[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(author.Ratings.ToList()[0].Id, authorDb.Ratings.ToList()[0].Id);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetById(author.Id, true, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(author.Name, authorDb.Name);
        Assert.Equal(author.Surname, authorDb.Surname);
        Assert.Equal(author.Email, authorDb.Email);
        Assert.Equal(author.Phone, authorDb.Phone);
        Assert.Equal(author.User!.Id, authorDb.User!.Id);
        Assert.Equal(author.Resources.ToList()[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(author.Comments.ToList()[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(author.Ratings.ToList()[0].Id, authorDb.Ratings.ToList()[0].Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByNameAsync(author.Name, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetByName(author.Name, false);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByNameAsync(author.Name, false);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetByName(author.Name, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.Equal(expecedState, _context.Entry(authorDb).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_props()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        var authorDb = await _authorRepository.GetByNameAsync(author.Name, true, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.NotNull(authorDb.Surname);
        Assert.NotNull(authorDb.Email);
        Assert.NotNull(authorDb.Phone);
        Assert.NotNull(authorDb.User!);
        Assert.NotNull(authorDb.Resources.ToList()[0]);
        Assert.NotNull(authorDb.Comments.ToList()[0]);
        Assert.NotNull(authorDb.Ratings.ToList()[0]);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_ItShould_entity_not_null_and_equals_props()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        var authorDb = _authorRepository.GetByName(author.Name, true, true);

        //assert
        Assert.NotNull(authorDb);
        Assert.Equal(author.Name, authorDb!.Name);
        Assert.NotNull(authorDb.Surname);
        Assert.NotNull(authorDb.Email);
        Assert.NotNull(authorDb.Phone);
        Assert.NotNull(authorDb.Resources.ToList()[0]);
        Assert.NotNull(authorDb.Comments.ToList()[0]);
        Assert.NotNull(authorDb.Ratings.ToList()[0]);
    }

    #endregion

    #region Update

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        //arrange
        var expectedName     = "new name";
        var expectedSurname  = "new surname";
        var expectedEmail    = "newEmail@mail.ru";
        var expectedPhone    = "+0 (987) 654-32-10";
        var expectedResources    = new List<Resource> { new() { Id    = Guid.NewGuid() } };
        var expectedComments = new List<Comment> { new() { Id = Guid.NewGuid() } };
        var expectedRatings  = new List<Rating> { new() { Id  = Guid.NewGuid() } };

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = expectedName,
            Surname = expectedSurname,
            Email = expectedEmail,
            Phone = expectedPhone,
            Resources = expectedResources,
            Comments = expectedComments,
            Ratings = expectedRatings,
        };
        await _authorRepository.AddAsync(author);
        await _context.SaveChangesAsync();

        //act
        author.Name = expectedName;
        await _authorRepository.UpdateAsync(author);
        await _context.SaveChangesAsync();

        var authorDb = await _authorRepository.GetByIdAsync(author.Id, true, true);

        //assert
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expectedName, authorDb.Name);
        Assert.Equal(expectedSurname, authorDb.Surname);
        Assert.Equal(expectedEmail, authorDb.Email);
        Assert.Equal(expectedPhone, authorDb.Phone);
        Assert.Equal(expectedResources[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(expectedResources.Count, authorDb.Resources.Count);
        Assert.Equal(expectedComments[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(expectedComments.Count, authorDb.Comments.Count);
        Assert.Equal(expectedRatings[0].Id, authorDb.Ratings.ToList()[0].Id);
        Assert.Equal(expectedRatings.Count, authorDb.Ratings.Count);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedName     = "new name";
        var expectedSurname  = "new surname";
        var expectedEmail    = "newEmail@mail.ru";
        var expectedPhone    = "+0 (987) 654-32-10";
        var expectedResources    = new List<Resource> { new() { Id    = Guid.NewGuid() } };
        var expectedComments = new List<Comment> { new() { Id = Guid.NewGuid() } };
        var expectedRatings  = new List<Rating> { new() { Id  = Guid.NewGuid() } };

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = expectedName,
            Surname = expectedSurname,
            Email = expectedEmail,
            Phone = expectedPhone,
            Resources = expectedResources,
            Comments = expectedComments,
            Ratings = expectedRatings,
        };
        _authorRepository.Add(author);
        _context.SaveChanges();

        //act
        author.Name = expectedName;
        _authorRepository.Update(author);
        _context.SaveChanges();

        var authorDb = _authorRepository.GetById(author.Id, true, true);

        //assert
        Assert.Equal(author.Id, authorDb!.Id);
        Assert.Equal(expectedName, authorDb.Name);
        Assert.Equal(expectedSurname, authorDb.Surname);
        Assert.Equal(expectedEmail, authorDb.Email);
        Assert.Equal(expectedPhone, authorDb.Phone);
        Assert.Equal(expectedResources[0].Id, authorDb.Resources.ToList()[0].Id);
        Assert.Equal(expectedResources.Count, authorDb.Resources.Count);
        Assert.Equal(expectedComments[0].Id, authorDb.Comments.ToList()[0].Id);
        Assert.Equal(expectedComments.Count, authorDb.Comments.Count);
        Assert.Equal(expectedRatings[0].Id, authorDb.Ratings.ToList()[0].Id);
        Assert.Equal(expectedRatings.Count, authorDb.Ratings.Count);
    }

    #endregion

    #region GetByHash

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByHashAsync_ItShould_null()
    {
        //arrange

        //act
        var authorDb = _authorRepository.GetByHashAsync(String.Empty, true, true);

        //assert
        Assert.Null(authorDb);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var authorDb = _authorRepository.GetByHash(String.Empty);

        //assert
        Assert.Null(authorDb);
    }

    #endregion
}