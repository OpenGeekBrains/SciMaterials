using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryTests.Helpers;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Tests;

public class CommentRepositoryTests
{
    private ICommentRepository _comentRepository;
    private SciMaterialsContext _context;

    public CommentRepositoryTests()
    {
        _context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        _comentRepository = new CommentRepository(_context, logger);
    }

    #region GetAll

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetAll_AsNoTracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedState = EntityState.Detached;

        //act
        var comments = _comentRepository.GetAll();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedState, _context.Entry(comments![0]).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetAll_Tracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var comments = _comentRepository.GetAll(false);
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(comments![0]).State);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetAllAsync_AsNoTracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Detached;

        //act
        var comments = await _comentRepository.GetAllAsync();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(comments![0]).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetAllAsync_Tracking_ItShould_contains_comment_1()
    {
        //arrange
        const int expected = 1;
        const EntityState expecedSstate = EntityState.Unchanged;

        //act
        var comments = await _comentRepository.GetAllAsync(false);
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        //assert
        Assert.Equal(expected, count);
        Assert.Equal(expecedSstate, _context.Entry(comments![0]).State);
    }

    #endregion

    #region Add

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void AddAsync_ItShould_contains_comment_increase_by_1()
    {
        //arrange
        var expected = (await _comentRepository.GetAllAsync())!.Count + 1;
        var comment = CommentHelper.GetOne();

        //act
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();

        var comments = await _comentRepository.GetAllAsync();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        var commentDb = await _comentRepository.GetByIdAsync(comment.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(comment.ParentId, commentDb!.ParentId);
        Assert.Equal(comment.AuthorId, commentDb!.AuthorId);
        Assert.Equal(comment.CreatedAt, commentDb.CreatedAt);
        Assert.Equal(comment.ResourceId, commentDb.ResourceId);
        Assert.Equal(comment.Author.Id, commentDb.Author.Id);
        Assert.Equal(comment.Resource!.Id, commentDb.Resource!.Id);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Add_ItShould_contains_comment_3()
    {
        //arrange
        var expected = _comentRepository.GetAll()!.Count + 1;
        var comment = CommentHelper.GetOne();

        //act
        _comentRepository.Add(comment);
        _context.SaveChanges();

        var comments = _comentRepository.GetAll();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        var commentDb = _comentRepository.GetById(comment.Id, include: true);

        //assert
        Assert.Equal(expected, count);
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(comment.ParentId, commentDb!.ParentId);
        Assert.Equal(comment.AuthorId, commentDb!.AuthorId);
        Assert.Equal(comment.CreatedAt, commentDb.CreatedAt);
        Assert.Equal(comment.ResourceId, commentDb.ResourceId);
        Assert.Equal(comment.Author.Id, commentDb.Author.Id);
        Assert.Equal(comment.Resource!.Id, commentDb.Resource!.Id);
    }

    #endregion

    #region Delete

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void DeleteAsync_ItShould_entity_removed()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();
        var expected = (await _comentRepository.GetAllAsync())!.Count - 1;

        //act
        await _comentRepository.DeleteAsync(comment.Id);
        await _context.SaveChangesAsync();

        var comments = await _comentRepository.GetAllAsync();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        var removedCategory = await _comentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedCategory);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Delete_ItShould_entity_removed()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        _comentRepository.Add(comment);
        _context.SaveChanges();
        var expected = _comentRepository.GetAll()!.Count - 1;

        //act
        _comentRepository.Delete(comment.Id);
        _context.SaveChanges();

        var comments = _comentRepository.GetAll();
        var count = 0;
        if (comments is not null)
            count = comments.Count;

        var removedCategory = _comentRepository.GetById(comment.Id);

        //assert
        Assert.Equal(expected, count);
        Assert.Null(removedCategory);
    }

    #endregion

    #region GetById

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_Tracking_ItShould_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var comment = CommentHelper.GetOne();
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();

        //act
        var commentDb = await _comentRepository.GetByIdAsync(comment.Id, false);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(expecedState, _context.Entry(commentDb).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_AsNoTracking_ItShould_no_tracking()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var comment = CommentHelper.GetOne();
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();

        //act
        var commentDb = await _comentRepository.GetByIdAsync(comment.Id, true);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(expecedState, _context.Entry(commentDb).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetById_Tracking_ItShould_unchanged()
    {
        //arrange
        const EntityState expecedState = EntityState.Unchanged;
        var comment = CommentHelper.GetOne();
        _comentRepository.Add(comment);
        _context.SaveChanges();

        //act
        var commentDb = _comentRepository.GetById(comment.Id, false);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(expecedState, _context.Entry(commentDb).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetByIdAsync_AsNoTracking_ItShould_Detached()
    {
        //arrange
        const EntityState expecedState = EntityState.Detached;
        var comment = CommentHelper.GetOne();
        _comentRepository.Add(comment);
        _context.SaveChanges();

        //act
        var commentDb = _comentRepository.GetById(comment.Id, true);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(expecedState, _context.Entry(commentDb).State);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public async void GetByIdAsync_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();

        //act
        var commentDb = await _comentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void GetById_ItShould_entity_not_null_and_equals_id()
    {
        //arrange
        var comment = CommentHelper.GetOne();
        _comentRepository.Add(comment);
        _context.SaveChanges();

        //act
        var commentDb = _comentRepository.GetById(comment.Id);

        //assert
        Assert.NotNull(commentDb);
        Assert.Equal(comment.Id, commentDb!.Id);
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
        //arrange
        var expectedCreatedAt = DateTime.Now.AddDays(1);
        var expectedText = "new some text";

        var comment = CommentHelper.GetOne();
        await _comentRepository.AddAsync(comment);
        await _context.SaveChangesAsync();

        //act
        comment.CreatedAt = expectedCreatedAt;
        comment.Text = expectedText;
        await _comentRepository.UpdateAsync(comment);
        await _context.SaveChangesAsync();

        var commentDb = await _comentRepository.GetByIdAsync(comment.Id);

        //assert
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(comment.Text, expectedText);
        Assert.Equal(comment.CreatedAt, expectedCreatedAt);
    }

    [Fact]
    [Trait("CommentRepositoryTests", nameof(Comment))]
    public void Update_ItShould_properties_updated()
    {
        //arrange
        var expectedCreatedAt = DateTime.Now.AddDays(1);
        var expectedText = "new some text";

        var comment = CommentHelper.GetOne();
        _comentRepository.Add(comment);
        _context.SaveChanges();

        //act
        comment.CreatedAt = expectedCreatedAt;
        comment.Text = expectedText;
        _comentRepository.Update(comment);
        _context.SaveChanges();

        var commentDb = _comentRepository.GetById(comment.Id);

        //assert
        Assert.Equal(comment.Id, commentDb!.Id);
        Assert.Equal(comment.Text, expectedText);
        Assert.Equal(comment.CreatedAt, expectedCreatedAt);
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
