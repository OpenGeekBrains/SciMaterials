using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.RepositoryLib.Repositories.UsersRepositories;

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
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public void AddAsync(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.AddAsync)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public void DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(UserRepository.DeleteAsync)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<User> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public Task<List<User>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetAllAsync)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public User GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public Task<User> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(UserRepository.GetByIdAsync)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.Update)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public void UpdateAsync(User entity)
    {
        _logger.Debug($"{nameof(UserRepository.UpdateAsync)}");
    }
}