using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;

namespace SciMaterials.RepositoryLib.Repositories.FilesRepositories;


/// <summary> Интерфейс репозитория для <see cref="Category"/>. </summary>
public interface ICategoryRepository : IRepository<Category> { }

/// <summary> Репозиторий для <see cref="Category"/>. </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ISciMaterialsContext _context;
    private readonly ILogger _logger;

    /// <summary> ctor. </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public CategoryRepository(
        ISciMaterialsContext context,
        ILogger logger)
    {
        _logger = logger;
        _logger.LogTrace($"Логгер встроен в {nameof(CategoryRepository)}");

        _context = context;
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Add"/>
    public void Add(Category entity)
    {
        _logger.LogInformation($"{nameof(Add)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(Add)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Categories.Add(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.AddAsync(T)"/>
    public async Task AddAsync(Category entity)
    {
        _logger.LogInformation($"{nameof(AddAsync)}");

        if (entity is null)
        {
            _logger.LogError($"{nameof(AddAsync)} >>> argumentNullException {nameof(entity)}");
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.Categories.AddAsync(entity);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Delete(T)"/>
    public void Delete(Category entity)
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
    public async Task DeleteAsync(Category entity)
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

        var categoryDb = _context.Categories.FirstOrDefault(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Categories.Remove(categoryDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.DeleteAsync(Guid)"/>
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(DeleteAsync)}");

        var categoryDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (categoryDb is null) return;
        _context.Categories.Remove(categoryDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAll(bool, bool)"/>
    public List<Category>? GetAll(bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories.Where(c => !c.IsDeleted);

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetAllAsync(bool, bool)"/>
    public async Task<List<Category>?> GetAllAsync(bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories.Where(c => !c.IsDeleted);

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetById(Guid, bool, bool)"/>
    public Category? GetById(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
                .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query
                .Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByIdAsync(Guid, bool, bool)"/>
    public async Task<Category?> GetByIdAsync(Guid id, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
                 .Where(c => c.Id == id && !c.IsDeleted);

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool, bool)"/>
    public async Task<Category?> GetByNameAsync(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
                .Where(c => c.Name == name && !c.IsDeleted);

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool, bool)"/>
    public Category? GetByName(string name, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
                .Where(c => c.Name == name && !c.IsDeleted)
                .AsQueryable();

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefault();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.Update"/>
    public void Update(Category entity)
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
        _context.Categories.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.UpdateAsync(T)"/>
    public async Task UpdateAsync(Category entity)
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
            _logger.LogError($"{nameof(Update)} >>> argumentNullException {nameof(entityDb)}");
            throw new ArgumentNullException(nameof(entityDb));
        }

        entityDb = UpdateCurrentEntity(entity, entityDb);
        _context.Categories.Update(entityDb);
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHashAsync(string, bool, bool)"/>
    public Task<Category?> GetByHashAsync(string hash, bool disableTracking = true, bool include = false) => null!;

    ///
    /// <inheritdoc cref="IRepository{T}.GetByHash(string, bool, bool)"/>
    public Category? GetByHash(string hash, bool disableTracking = true, bool include = false) => null;

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
    public List<Category>? GetPage(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (include)
            query = query.Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetPageAsync(int, int, bool, bool)"/>
    public async Task<List<Category>?> GetPageAsync(int pageNumb, int pageSize, bool disableTracking = true, bool include = false)
    {
        var query = _context.Categories
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (include)
            query = query
                .Include(c => c.Resources);

        if (disableTracking)
            query = query.AsNoTracking();

        return await query
            .Skip((pageNumb - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    /// <summary> Обновить данные экземпляра каегории. </summary>
    /// <param name="source"> Источник. </param>
    /// <param name="recipient"> Получатель. </param>
    /// <returns> Обновленный экземпляр. </returns>
    private Category UpdateCurrentEntity(Category source, Category recipient)
    {
        recipient.Description = source.Description;
        recipient.CreatedAt = source.CreatedAt;
        recipient.Resources = source.Resources;;
        recipient.ParentId = source.ParentId;
        recipient.Name = source.Name;
        recipient.IsDeleted = source.IsDeleted;

        return recipient;
    }
}