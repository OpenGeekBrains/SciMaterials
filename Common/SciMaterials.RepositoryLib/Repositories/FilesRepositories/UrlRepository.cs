using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;

using Url = SciMaterials.DAL.Models.Url;

namespace SciMaterials.RepositoryLib.Repositories.UrlsRepositories;

/// <summary> Интерфейс репозитория для <see cref="Url"/>. </summary>
public interface IUrlRepository : IRepository<Url> { }

/// <summary> Репозиторий для <see cref="Url"/>. </summary>
public class UrlRepository : IUrlRepository
{
    private readonly ISciMaterialsContext _context;
    private readonly ILogger _logger;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public UrlRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(UrlRepository)}");
        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Url entity)
    {
        _logger.LogInformation($"{nameof(Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Urls.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Url entity)
    {
        _logger.LogInformation($"{nameof(AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Set<Url>().AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Url entity)
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
    public async Task DeleteAsync(Url entity)
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

        var entityDb = _context.Urls.FirstOrDefault(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(Delete)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Urls.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)}");

        var entityDb = await _context.Urls.FirstOrDefaultAsync(c => c.Id == id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(DeleteAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        _context.Urls.Remove(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll(bool, bool)"/>
    public List<Url>? GetAll(bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public async Task<List<Url>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public Url? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(c => c.Id == id && !c.IsDeleted)
            .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public async Task<Url?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(c => c.Id == id)
            .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public void Update(Url entity)
    {
        _logger.LogInformation($"{nameof(Update)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = _context.Set<Url>().Find(entity.Id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Urls.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Url entity)
    {
        _logger.LogInformation($"{nameof(UpdateAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(UpdateAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        var entityDb = await _context.Set<Url>().FindAsync(entity.Id);

        if (entityDb is null)
        {
            _logger.LogError($"{nameof(UpdateAsync)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Urls.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool, bool)"/>
    public async Task<Url?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
                .Where(c => c.Name == name && !c.IsDeleted)
                .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public Url? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
                .Where(c => c.Name == name && !c.IsDeleted)
                .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public Task<Url?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null!;

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
    public List<Url>? GetPage(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query.Include(f => f.Categories)
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
    public async Task<List<Url>?> GetPageAsync(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Urls
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (include)
            query = query
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
    public Url? GetByHash(string hash, bool disableTracking = true, bool include = false) => null!;

    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="sourse"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Url UpdateCurrentEntity(Url sourse, Url recipient)
    {
        recipient.Name = sourse.Name;
        recipient.IsDeleted = sourse.IsDeleted;
        recipient.ShortInfo = sourse.ShortInfo;
        recipient.Description = sourse.Description;
        recipient.AuthorId = sourse.AuthorId;
        recipient.CreatedAt = sourse.CreatedAt;
        recipient.Author = sourse.Author;
        recipient.Comments = sourse.Comments;
        recipient.Tags = sourse.Tags;
        recipient.Categories = sourse.Categories;
        recipient.Ratings = sourse.Ratings;

        return recipient;
    }
}