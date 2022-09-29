
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.DAL.Models;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.Repositories.CategorysRepositories;


/// <summary> Интерфейс репозитория для <see cref="Category"/>. </summary>
public interface ICategoryRepository : IRepository<Category> { }

/// <summary> Репозиторий для <see cref="Category"/>. </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public CategoryRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(CategoryRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Category entity)
    {
        _logger.Debug($"{nameof(CategoryRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(CategoryRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<Category> GetAll()
    {
        _logger.Debug($"{nameof(CategoryRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.GetById(Guid)"/>
    public Category GetById(Guid id)
    {
        _logger.Debug($"{nameof(CategoryRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Category entity)
    {
        _logger.Debug($"{nameof(CategoryRepository.Update)}");
    }
}