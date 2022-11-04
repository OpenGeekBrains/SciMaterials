using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.RepositoryLib.Repositories.FilesRepositories;

/// <summary> Интерфейс репозитория для <see cref="ContentType"/>. </summary>
public interface IContentTypeRepository : IRepository<ContentType> { }

/// <summary> Репозиторий для <see cref="ContentType"/>. </summary>
public class ContentTypeRepository : IContentTypeRepository
{
    private readonly ISciMaterialsContext _context;
    private readonly ILogger _logger;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public ContentTypeRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(ContentTypeRepository)}");
        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(ContentType entity)
    {
        _logger.LogInformation($"{nameof(Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.ContentTypes.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(ContentType entity)
    {
        _logger.LogInformation($"{nameof(AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.ContentTypes.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(ContentType entity)
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
    public async Task DeleteAsync(ContentType entity)
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

        var ContentTypeDb = _context.ContentTypes.FirstOrDefault(c => c.Id == id);

        if (ContentTypeDb is null)
        {
            _logger.LogError($"{nameof(Delete)} >>> argumentNullException {nameof(ContentTypeDb)}");
            throw new ArgumentNullException(nameof(ContentTypeDb));
        }

        _context.ContentTypes.Remove(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)}");

        var ContentTypeDb = await _context.ContentTypes.FirstOrDefaultAsync(c => c.Id == id);

        if (ContentTypeDb is null)
        {
            _logger.LogError($"{nameof(DeleteAsync)} >>> argumentNullException {nameof(ContentTypeDb)}");
            throw new ArgumentNullException(nameof(ContentTypeDb));
        }

        _context.ContentTypes.Remove(ContentTypeDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll(bool, bool)"/>
    public List<ContentType>? GetAll(bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes.Where(c => !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool, bool)"/>
    public async Task<List<ContentType>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes.Where(c => !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool, bool)"/>
    public ContentType? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool, bool)"/>
    public async Task<ContentType?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(ContentType entity)
    {
        _logger.LogInformation($"{nameof(Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = GetById(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.ContentTypes.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(ContentType entity)
    {
        _logger.LogInformation($"{nameof(UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await GetByIdAsync(entity.Id, false);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.ContentTypes.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool, bool)"/>
    public async Task<ContentType?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool, bool)"/>
    public ContentType? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query = query.Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool, bool)"/>
    public Task<ContentType?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null!;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool, bool)"/>
    public ContentType? GetByHash(string hash, bool disableTracking = true, bool include = false) => null;

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
    public List<ContentType>? GetPage(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes.AsQueryable();

        if (include)
            query = query
                .Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetPageAsync(int, int, bool, bool)"/>
    public async Task<List<ContentType>?> GetPageAsync(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        IQueryable<ContentType> query = _context.ContentTypes.AsQueryable();

        if (include)
            query = query
                .Include(ct => ct.Files);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }



    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private ContentType UpdateCurrentEntity(ContentType sourse, ContentType recipient)
    {
        recipient.Files = sourse.Files;
        recipient.Name = sourse.Name;
        recipient.FileExtension = sourse.FileExtension;
        recipient.IsDeleted = sourse.IsDeleted;

        return recipient;
    }
}