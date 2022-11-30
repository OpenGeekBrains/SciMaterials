using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class CommentRepositoryTests
{
    private readonly CommentRepository _ComentRepository;
    private readonly SciMaterialsContext _Context;

    public CommentRepositoryTests()
    {
        _Context = SciMateralsContextHelper.Create();

        var logger = new Mock<ILogger<CommentRepository>>();
        _ComentRepository = new CommentRepository(_Context, logger.Object);
    }

    #region GetAll

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetAll_AsNoTracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expected_state = EntityState.Detached;

        //act
        var comments = _ComentRepository.GetAll();
        var count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expected_state, _Context.Entry(comments[0]).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetAll_Tracking_ItShould_contains_comment_1()
    {
        _ComentRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var comments = _ComentRepository.GetAll();
        var count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(comments[0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Detached;

        //act
        var comments = await _ComentRepository.GetAllAsync();
        var count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(comments[0]).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetAllAsync_Tracking_ItShould_contains_comment_1()
    {
        _ComentRepository.NoTracking = false;

        //arrange
        const int expected = 1;
        const EntityState expeced_sstate = EntityState.Unchanged;

        //act
        var comments = await _ComentRepository.GetAllAsync();
        var count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expeced_sstate, _Context.Entry(comments[0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void AddAsync_ItShould_contains_comment_increase_by_1()
    {
        _ComentRepository.Include = true;

        //arrange
        var all = await _ComentRepository.GetAllAsync();
        var expected  = all.Count + 1;
        var comment   = CommentHelper.GetOne();

        //act
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();

        var comments = await _ComentRepository.GetAllAsync();
        var count = comments.Count;

        var comment_db = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(comment.ParentId, comment_db.ParentId);
        Assert.Equal(comment.AuthorId, comment_db.AuthorId);
        Assert.Equal(comment.CreatedAt, comment_db.CreatedAt);
        Assert.Equal(comment.ResourceId, comment_db.ResourceId);
        Assert.Equal(comment.Author.Id, comment_db.Author.Id);
        Assert.Equal(comment.Resource.Id, comment_db.Resource.Id);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Add_ItShould_contains_comment_3()
    {
        _ComentRepository.Include = true;

        //arrange
        var expected = _ComentRepository.GetAll().Count + 1;
        var comment  = CommentHelper.GetOne();

        //act
        _ComentRepository.Add(comment);
        _Context.SaveChanges();

        var comments = _ComentRepository.GetAll();
        var count = comments.Count;

        var comment_db = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(comment.ParentId, comment_db.ParentId);
        Assert.Equal(comment.AuthorId, comment_db.AuthorId);
        Assert.Equal(comment.CreatedAt, comment_db.CreatedAt);
        Assert.Equal(comment.ResourceId, comment_db.ResourceId);
        Assert.Equal(comment.Author.Id, comment_db.Author.Id);
        Assert.Equal(comment.Resource.Id, comment_db.Resource.Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();
        var expected = (await _ComentRepository.GetAllAsync()).Count - 1;

        //act
        await _ComentRepository.DeleteAsync(comment.Id);
        await _Context.SaveChangesAsync();

        var comments = await _ComentRepository.GetAllAsync();
        var count = comments.Count;

        var removed_category = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_category);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        _ComentRepository.Add(comment);
        _Context.SaveChanges();
        var expected = _ComentRepository.GetAll().Count - 1;

        //act
        _ComentRepository.Delete(comment.Id);
        _Context.SaveChanges();

        var comments = _ComentRepository.GetAll();
        var count = comments.Count;

        var removed_category = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removed_category);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        _ComentRepository.NoTracking = false;

        //arrange
        const EntityState expected_state = EntityState.Unchanged;
        var comment = CommentHelper.GetOne();
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();

        //act
        var comment_db = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(expected_state, _Context.Entry(comment_db).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var comment = CommentHelper.GetOne();
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();

        //act
        var comment_db = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(expected_state, _Context.Entry(comment_db).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange

        _ComentRepository.NoTracking = false;

        const EntityState expected_state = EntityState.Unchanged;
        var comment = CommentHelper.GetOne();
        _ComentRepository.Add(comment);
        _Context.SaveChanges();

        //act
        var comment_db = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(expected_state, _Context.Entry(comment_db).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expected_state = EntityState.Detached;
        var comment = CommentHelper.GetOne();
        _ComentRepository.Add(comment);
        _Context.SaveChanges();

        //act
        var comment_db = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
        Assert.Equal(expected_state, _Context.Entry(comment_db).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();

        //act
        var comment_db = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        _ComentRepository.Add(comment);
        _Context.SaveChanges();

        //act
        var comment_db = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.NotNull(comment_db);
        Assert.Equal(comment.Id, comment_db.Id);
    }

    #endregion

    #region GetByName данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public async void GetByNameAsync_AsNoTracking_ItShould_detached()
    //{

    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public void GetByName_Tracking_ItShould_unchanged()
    //{
        
    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public async void GetByNameAsync_Tracking_ItShould_unchanged()
    //{
        
    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public void GetByName_AsNoTracking_ItShould_detached()
    //{
        
    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public async void GetByNameAsync_ItShould_entity_not_null_and_equals_name()
    //{
        
    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public void GetByName_ItShould_entity_not_null_and_equals_name()
    //{
        
    //}

    #endregion

    #region Update

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void UpdateAsync_ItShould_properties_updated()
    {
        _ComentRepository.NoTracking = false;

        //arrange
        var expected_created_at = DateTime.Now.AddDays(1);
        var expected_text = "new some text";

        var comment = CommentHelper.GetOne();
        await _ComentRepository.AddAsync(comment);
        await _Context.SaveChangesAsync();

        //act
        comment.CreatedAt = expected_created_at;
        comment.Text = expected_text;
        await _ComentRepository.UpdateAsync(comment);
        await _Context.SaveChangesAsync();

        var comment_db = await _ComentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.Equal(comment.Id, comment_db!.Id);
        Assert.Equal(comment.Text, expected_text);
        Assert.Equal(comment.CreatedAt, expected_created_at);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Update_ItShould_properties_updated()
    {
        _ComentRepository.NoTracking = false;

        //arrange
        var expected_created_at = DateTime.Now.AddDays(1);
        var expected_text = "new some text";

        var comment = CommentHelper.GetOne();
        _ComentRepository.Add(comment);
        _Context.SaveChanges();

        //act
        comment.CreatedAt = expected_created_at;
        comment.Text = expected_text;
        _ComentRepository.Update(comment);
        _Context.SaveChanges();

        var comment_db = _ComentRepository.GetById(comment.Id);

        //assert
        Assert.Equal(comment.Id, comment_db!.Id);
        Assert.Equal(comment.Text, expected_text);
        Assert.Equal(comment.CreatedAt, expected_created_at);
    }

    #endregion

    #region GetByHash данные методы в репозитории не реализованы

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public async void GetByHashAsync_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var commentDb = await _comentRepository.GetByHashAsync(String.Empty);

    //    //assert
    //    Assert.Null(commentDb);
    //}

    //[Fact]
    //[Trait("CommentRepositoryTests", nameof(Comment))]
    //public void GetByHash_ItShould_null()
    //{
    //    //arrange

    //    //act
    //    var commentDb = _comentRepository.GetByHash(String.Empty);

    //    //assert
    //    Assert.Null(commentDb);
    //}

    #endregion
}
