using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryLib.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="File"/>. </summary>
public interface IFileRepository : IRepository<File> { }

/// <summary> Репозиторий для <see cref="File"/>. </summary>
public class FileRepository : IFileRepository
{
    private readonly ISciMaterialsContext _context;
    private readonly ILogger _logger;

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
        _logger.LogInformation($"{nameof(Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Files.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(File entity)
    {
        _logger.LogInformation($"{nameof(AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Set<File>().AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(File entity)
    {
        _logger.LogInformation($"{nameof(Delete)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Delete)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        Delete(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(T)"/>
    public async Task DeleteAsync(File entity)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(DeleteAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.Id == default) return;
        await DeleteAsync(entity.Id);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(Guid)"/>
    public void Delete(Guid id)
    {
        _logger.LogInformation($"{nameof(Delete)}");

        var entityDb = _context.Files.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Files.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)}");

        var entityDb = await _context.Files.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Files.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll(bool, bool)"/>
    public List<File>? GetAll(bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(f => !f.IsDeleted)
            .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool, bool)"/>
    public async Task<List<File>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(f => !f.IsDeleted)
            .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool, bool)"/>
    public File? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(c => c.Id == id && !c.IsDeleted)
            .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool, bool)"/>
    public async Task<File?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(c => c.Id == id)
            .AsQueryable();

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
        _logger.LogInformation($"{nameof(Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Files.Update(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(File entity)
    {
        _logger.LogInformation($"{nameof(UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Files.Update(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool, bool)"/>
    public async Task<File?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
                .Where(c => c.Name == name && !c.IsDeleted)
                .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool, bool)"/>
    public File? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
                .Where(c => c.Name == name && !c.IsDeleted)
                .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool, bool)"/>
    public async Task<File?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
                .Where(c => c.Hash == hash && !c.IsDeleted)
                .AsQueryable();

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
    /// <inheritdoc cref="IRepository{T}.GetCount()"/>
    public int GetCount()
        => _context.Categories.Count();

    ///
    /// <inheritdoc cref="IRepository{T}.GetCountAsync()"/>
    public async Task<int> GetCountAsync()
        => await _context.Categories.CountAsync();

    ///
    /// <inheritdoc cref="IRepository{T}.GetPage(int, int, bool, bool)"/>
    public List<File>? GetPage(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetPageAsync(int, int, bool, bool)"/>
    public async Task<List<File>?> GetPageAsync(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query
                .Include(f => f.ContentType)
                .Include(f => f.FileGroup)
                .Include(f => f.Categories)
                .Include(f => f.Author)
                .Include(f => f.Comments)
                .Include(f => f.Tags)
                .Include(f => f.Ratings);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool, bool)"/>
    public File? GetByHash(string hash, bool disableTracking = true, bool include = false)
    {
        var query = _context.Files
                .Where(c => c.Hash == hash && !c.IsDeleted)
                .AsQueryable();

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