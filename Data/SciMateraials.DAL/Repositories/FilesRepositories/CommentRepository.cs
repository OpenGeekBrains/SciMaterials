
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.CommentsRepositories;

/// <summary> Интерфейс репозитория для <see cref="Comment"/>. </summary>
public interface ICommentRepository : IRepository<Comment> { }

/// <summary> Репозиторий для <see cref="Comment"/>. </summary>
public class CommentRepository : ICommentRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public CommentRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(CommentRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Comment entity)
    {
        _logger.Debug($"{nameof(CommentRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(CommentRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Comment> GetAll()
    {
        _logger.Debug($"{nameof(CommentRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.GetById(Guid)"/>
    public Comment GetById(Guid id)
    {
        _logger.Debug($"{nameof(CommentRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Comment entity)
    {
        _logger.Debug($"{nameof(CommentRepository.Update)}");
    }
}