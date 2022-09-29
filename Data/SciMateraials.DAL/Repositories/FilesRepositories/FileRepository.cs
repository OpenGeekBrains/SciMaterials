
using Microsoft.EntityFrameworkCore;
using NLog;
using SciMaterials.Data.Repositories;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="File"/>. </summary>
public interface IFileRepository : IRepository<File> { }

/// <summary> Репозиторий для <see cref="File"/>. </summary>
public class FileRepository : IFileRepository
{
    private readonly ILogger _logger;
    private readonly DbContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public FileRepository(
        DbContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(FileRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(File entity)
    {
        _logger.Debug($"{nameof(FileRepository.Add)}");
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.Debug($"{nameof(FileRepository.Delete)}");
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<File> GetAll()
    {
        _logger.Debug($"{nameof(FileRepository.GetAll)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepositoryGuid{T}.GetById(Guid)"/>
    public File GetById(Guid id)
    {
        _logger.Debug($"{nameof(FileRepository.GetById)}");



        return null!;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(File entity)
    {
        _logger.Debug($"{nameof(FileRepository.Update)}");
    }
}