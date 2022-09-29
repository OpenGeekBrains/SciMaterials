using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Models;

namespace SciMaterials.Data.Repositories.UserRepositories;

/// <summary> Интерфейс репозитория для <see cref="User"/>. </summary>
public interface IUserRepository : IRepository<User> { }

/// <summary> Репозиторий для <see cref="User"/>. </summary>
public class UserRepository : IUserRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public UserRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(UserRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<User> GetAll()
    {
        _logger.Debug($"{nameof(UserRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.GetById(Guid)"/>
    public User GetById(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.Update)}");
    }
}