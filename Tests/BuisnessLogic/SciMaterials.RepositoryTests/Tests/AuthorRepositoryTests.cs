using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Users;
using SciMaterials.DAL.Resources.Extensions;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class AuthorRepositoryTests
{
    private readonly AuthorRepository _AuthorRepository;
    private readonly SciMaterialsContext _Context;

    public AuthorRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<AuthorRepository>>();
        _AuthorRepository = new AuthorRepository(_Context, logger.Object);
    }

    #region GetAll

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int         expected       = 1;
        const EntityState expected_state = EntityState.Detached;

        //act
        var authors = _AuthorRepository.GetAll();
        var count   = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(authors[0]).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetAll_Tracking_ItShould_contains_category_1()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const int         expected       = 1;
        const EntityState expected_state = EntityState.Unchanged;

        //act
        var authors = _AuthorRepository.GetAll();
        var count   = authors.Count;

        //assert
        Assert.Equal(expected, count);
        var entity_state = _Context.Entry(authors[0]).State;
        Assert.Equal(expected_state, entity_state);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_category_1()
    {
        //arrange
        const int         expected       = 1;
        const EntityState expected_state = EntityState.Detached;

        //act
        var authors = await _AuthorRepository.GetAllAsync();
        var count   = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(authors[0]).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetAllAsync_Tracking_ItShould_contains_category_1()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const int         expected       = 1;
        const EntityState expected_state = EntityState.Unchanged;

        //act
        var authors = await _AuthorRepository.GetAllAsync();

        var count = authors.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(authors[0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void AddAsync_ItShould_contains_category_increase_by_1()
    {
        _AuthorRepository.Include = true;

        //arrange
        var all_authors = await _AuthorRepository.GetAllAsync();
        var expected    = all_authors.Count + 1;
        var author      = AuthorHelper.GetOne();

        //act
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        var authors = await _AuthorRepository.GetAllAsync();
        var count   = authors.Count;

        var author_db = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(author.Surname, author_db.Surname);
        Assert.Equal(author.Email, author_db.Email);
        Assert.Equal(author.Phone, author_db.Phone);
        Assert.Equal(author.User!.Id, author_db.User!.Id);
        Assert.Equal(author.Resources.First().Id, author_db.Resources.First().Id);
        Assert.Equal(author.Comments.First().Id, author_db.Comments.First().Id);
        Assert.Equal(author.Ratings.First().Id, author_db.Ratings.First().Id);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Add_ItShould_contains_category_3()
    {
        _AuthorRepository.Include = true;

        //arrange
        var expected = _AuthorRepository.GetAll().Count + 1;
        var author   = AuthorHelper.GetOne();

        //act
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        var authors = _AuthorRepository.GetAll();
        var count   = authors.Count;

        var author_db = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(author.Surname, author_db.Surname);
        Assert.Equal(author.Email, author_db.Email);
        Assert.Equal(author.Phone, author_db.Phone);
        Assert.Equal(author.User!.Id, author_db.User!.Id);
        Assert.Equal(author.Resources.First().Id, author_db.Resources.First().Id);
        Assert.Equal(author.Comments.First().Id, author_db.Comments.First().Id);
        Assert.Equal(author.Ratings.First().Id, author_db.Ratings.First().Id);
    }

    #endregion

    #region Delete

    [Fact(Skip = "Требуется переработка для случаев с наличием связных сущностей")]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();
        var all = await _AuthorRepository.GetAllAsync();
        var expected  = all.Count - 1;

        //act
        await _AuthorRepository.DeleteAsync(author.Id);
        await _Context.SaveChangesAsync();

        var authors = await _AuthorRepository.GetAllAsync();
        var count   = authors.Count;

        var removed_author = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_author);
    }

    [Fact(Skip = "Требуется переработка для случаев с наличием связных сущностей")]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var author = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();
        var all     = _AuthorRepository.GetAll();
        var expected = all.Count - 1;

        //act
        _AuthorRepository.Delete(author.Id);
        _Context.SaveChanges();

        var authors = _AuthorRepository.GetAll();
        var count   = authors.Count;

        var removed_author = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_author);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var               author       = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var               author       = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var               author       = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var               author       = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        _AuthorRepository.Include = true;

        //arrange
        var author = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(author.Surname, author_db.Surname);
        Assert.Equal(author.Email, author_db.Email);
        Assert.Equal(author.Phone, author_db.Phone);
        Assert.Equal(author.User!.Id, author_db.User!.Id);
        Assert.Equal(author.Resources.First().Id, author_db.Resources.First().Id);
        Assert.Equal(author.Comments.First().Id, author_db.Comments.First().Id);
        Assert.Equal(author.Ratings.First().Id, author_db.Ratings.First().Id);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        _AuthorRepository.Include = true;

        //arrange
        var author = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Id, author_db.Id);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(author.Surname, author_db.Surname);
        Assert.Equal(author.Email, author_db.Email);
        Assert.Equal(author.Phone, author_db.Phone);
        Assert.Equal(author.User!.Id, author_db.User!.Id);
        Assert.Equal(author.Resources.First().Id, author_db.Resources.First().Id);
        Assert.Equal(author.Comments.First().Id, author_db.Comments.First().Id);
        Assert.Equal(author.Ratings.First().Id, author_db.Ratings.First().Id);
    }

    #endregion

    #region GetByName

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var               author       = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByNameAsync(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_Tracking_ItShould_unchanged()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var               author       = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetByName(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_Tracking_ItShould_unchanged()
    {
        _AuthorRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var               author       = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByNameAsync(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_AsNoTracking_ItShould_detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var               author       = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetByName(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.Equal(expected_state, _Context.Entry(author_db).State);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void GetByNameAsync_ItShould_entity_not_null_and_equals_props()
    {
        _AuthorRepository.Include = true;

        //arrange
        var author = AuthorHelper.GetOne();
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        var author_db = await _AuthorRepository.GetByNameAsync(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.NotNull(author_db.Surname);
        Assert.NotNull(author_db.Email);
        Assert.NotNull(author_db.Phone);
        Assert.NotNull(author_db.User!);
        Assert.NotNull(author_db.Resources.First());
        Assert.NotNull(author_db.Comments.First());
        Assert.NotNull(author_db.Ratings.First());
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByName_ItShould_entity_not_null_and_equals_props()
    {
        _AuthorRepository.Include = true;

        //arrange
        var author = AuthorHelper.GetOne();
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        var author_db = _AuthorRepository.GetByName(author.Name);

        //assert
        Assert.NotNull(author_db);
        Assert.Equal(author.Name, author_db.Name);
        Assert.NotNull(author_db.Surname);
        Assert.NotNull(author_db.Email);
        Assert.NotNull(author_db.Phone);
        Assert.NotNull(author_db.Resources.First());
        Assert.NotNull(author_db.Comments.First());
        Assert.NotNull(author_db.Ratings.First());
    }

    #endregion

    #region Update

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _AuthorRepository.NoTracking = false;
        _AuthorRepository.Include = true;

        //arrange
        var expected_name      = "new name";
        var expected_surname   = "new surname";
        var expected_email     = "newEmail@mail.ru";
        var expected_phone     = "+0 (987) 654-32-10";
        var expected_resources = new List<Resource> { new() { Id = Guid.NewGuid() } };
        var expected_comments  = new List<Comment> { new() { Id  = Guid.NewGuid() } };
        var expected_ratings   = new List<Rating> { new() { Id   = Guid.NewGuid() } };

        var author = new Author
        {
            Id        = Guid.NewGuid(),
            Name      = expected_name,
            Surname   = expected_surname,
            Email     = expected_email,
            Phone     = expected_phone,
            Resources = expected_resources,
            Comments  = expected_comments,
            Ratings   = expected_ratings,
        };
        await _AuthorRepository.AddAsync(author);
        await _Context.SaveChangesAsync();

        //act
        author.Name = expected_name;
        await _AuthorRepository.UpdateAsync(author);
        await _Context.SaveChangesAsync();

        var author_db = await _AuthorRepository.GetByIdAsync(author.Id);

        //assert
        Assert.Equal(author.Id, author_db!.Id);
        Assert.Equal(expected_name, author_db.Name);
        Assert.Equal(expected_surname, author_db.Surname);
        Assert.Equal(expected_email, author_db.Email);
        Assert.Equal(expected_phone, author_db.Phone);
        Assert.Equal(expected_resources[0].Id, author_db.Resources.First().Id);
        Assert.Equal(expected_resources.Count, author_db.Resources.Count);
        Assert.Equal(expected_comments[0].Id, author_db.Comments.First().Id);
        Assert.Equal(expected_comments.Count, author_db.Comments.Count);
        Assert.Equal(expected_ratings[0].Id, author_db.Ratings.First().Id);
        Assert.Equal(expected_ratings.Count, author_db.Ratings.Count);
    }

    [Fact]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void Update_ItShould_properties_updated()
    {
        _AuthorRepository.Include = true;
        _AuthorRepository.NoTracking = false;

        //arrange
        var expected_name      = "new name";
        var expected_surname   = "new surname";
        var expected_email     = "newEmail@mail.ru";
        var expected_phone     = "+0 (987) 654-32-10";
        var expected_resources = new List<Resource> { new() { Id = Guid.NewGuid() } };
        var expected_comments  = new List<Comment> { new() { Id  = Guid.NewGuid() } };
        var expected_ratings   = new List<Rating> { new() { Id   = Guid.NewGuid() } };

        var author = new Author
        {
            Id        = Guid.NewGuid(),
            Name      = "old_name",
            Surname   = expected_surname,
            Email     = expected_email,
            Phone     = expected_phone,
            Resources = expected_resources,
            Comments  = expected_comments,
            Ratings   = expected_ratings,
        };
        _AuthorRepository.Add(author);
        _Context.SaveChanges();

        //act
        author.Name = expected_name;
        _AuthorRepository.Update(author);
        _Context.SaveChanges();

        var author_db = _AuthorRepository.GetById(author.Id);

        //assert
        Assert.Equal(author.Id, author_db!.Id);
        Assert.Equal(expected_name, author_db.Name);
        Assert.Equal(expected_surname, author_db.Surname);
        Assert.Equal(expected_email, author_db.Email);
        Assert.Equal(expected_phone, author_db.Phone);
        Assert.Equal(expected_resources[0].Id, author_db.Resources.First().Id);
        Assert.Equal(expected_resources.Count, author_db.Resources.Count);
        Assert.Equal(expected_comments[0].Id, author_db.Comments.First().Id);
        Assert.Equal(expected_comments.Count, author_db.Comments.Count);
        Assert.Equal(expected_ratings[0].Id, author_db.Ratings.First().Id);
        Assert.Equal(expected_ratings.Count, author_db.Ratings.Count);
    }

    #endregion

    #region GetByHash

    [Fact(Skip = "Не реализован метод")]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByHashAsync_ItShould_null()
    {
        _AuthorRepository.Include = true;

        //arrange

        //act
        var author_db = _AuthorRepository.GetByHashAsync(string.Empty);

        //assert
        Assert.Null(author_db);
    }

    [Fact(Skip = "Не реализован метод")]
    [Trait("AuthorRepositoryTests", nameof(Author))]
    public void GetByHash_ItShould_null()
    {
        //arrange

        //act
        var author_db = _AuthorRepository.GetByHash(string.Empty);

        //assert
        Assert.Null(author_db);
    }

    #endregion
}
