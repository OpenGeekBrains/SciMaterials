
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.RatingRepositories;


/// <summary> Интерфейс репозитория для <see cref="Rating"/>. </summary>
public interface IRatingRepository : IRepository<Rating> { }

/// <summary> Репозиторий для <see cref="Rating"/>. </summary>
public class RatingRepository : IRatingRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public RatingRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(RatingRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public void AddAsync(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.AddAsync)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(RatingRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public void DeleteAsync(Guid id)
    {
        _logger.Debug($"{nameof(RatingRepository.DeleteAsync)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Rating> GetAll(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public Task<List<Rating>> GetAllAsync(bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetAllAsync)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public Rating GetById(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public Task<Rating> GetByIdAsync(Guid id, bool disableTracking = true)
    {
        _logger.Debug($"{nameof(RatingRepository.GetByIdAsync)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.Update)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public void UpdateAsync(Rating entity)
    {
        _logger.Debug($"{nameof(RatingRepository.UpdateAsync)}");
    }
}