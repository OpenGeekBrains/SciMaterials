﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Repositories.CategorysRepositories;
using SciMaterials.Data.Repositories;
using System.Xml.Linq;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="File"/>. </summary>
public interface IFileRepository : IRepository<File> { }

/// <summary> Репозиторий для <see cref="File"/>. </summary>
public class FileRepository : IFileRepository
{
    private readonly ILogger _logger;
    private readonly ISciMaterialsContext _context;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public FileRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(FileRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Files.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Set<File>().AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(FileRepository.Delete)}");

        var entityDb = _context.Files.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileRepository.Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Files.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(FileRepository.DeleteAsync)}");

        var entityDb = await _context.Files.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileRepository.DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Files.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll"/>
    public List<File>? GetAll(bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files.Where(f => !f.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool)"/>
    public async Task<List<File>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files.Where(f => !f.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool)"/>
    public File? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
            .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool)"/>
    public async Task<File?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
            .Where(c => c.Id == id);
        
        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = _context.Set<File>().Find(entity.Id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileRepository.Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Entry(entityDb).CurrentValues.SetValues(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(File entity)
    {
        _logger.LogInformation($"{nameof(FileRepository.UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(FileRepository.UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await _context.Set<File>().FindAsync(entity.Id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(FileRepository.UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Entry(entityDb).CurrentValues.SetValues(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool)"/>
    public async Task<File?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool)"/>
    public File? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool)"/>
    public async Task<File?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
                .Where(c => c.Hash == hash && !c.IsDeleted);
        
        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool)"/>
    public File? GetByHash(string hash, bool disableTracking = true, bool include = false)
    {
        IQueryable<File> query = _context.Files
                .Where(c => c.Hash == hash && !c.IsDeleted);

        if (include)
            query = query.Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }
}