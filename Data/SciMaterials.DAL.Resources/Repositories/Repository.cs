using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contracts.Entities;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories;

namespace SciMaterials.DAL.Resources.Repositories;

public abstract class Repository<T> : IDbRepository<T> where T : BaseModel
{
    protected readonly DbSet<T> _Set;

    internal SciMaterialsContext Context { get; }

    internal ILogger<Repository<T>> Logger { get; }

    public virtual IQueryable<T> Items
    {
        get
        {
            var query = NoTracking ? _Set.AsNoTracking() : _Set;
            return Include ? GetIncludeQuery(query) : query;
        }
    }

    public virtual IQueryable<T> ItemsNotDeleted => Items.Where(item => !item.IsDeleted);

    public bool NoTracking { get; set; } = true;

    public bool Include { get; set; }

    protected Repository(SciMaterialsContext Context, ILogger<Repository<T>> Logger)
    {
        this.Context = Context;
        this.Logger  = Logger;
        _Set         = Context.Set<T>();
    }

    protected virtual IQueryable<T> GetIncludeQuery(IQueryable<T> query) => query;

    public virtual int GetCount()
    {
        var count = ItemsNotDeleted.Count();
        return count;
    }

    public async Task<int> GetCountAsync()
    {
        var count = await ItemsNotDeleted.CountAsync();
        return count;
    }

    public virtual void Add(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _Set.Add(entity);
    }

    public virtual async Task AddAsync(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        await _Set.AddAsync(entity);
    }

    public virtual void Delete(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        if (entity.Id == default) return;

        Delete(entity.Id);
    }

    public virtual async Task DeleteAsync(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        if (entity.Id == default) return;

        await DeleteAsync(entity.Id);
    }

    public virtual void Delete(Guid Id)
    {
        var db_item = _Set.FirstOrDefault(c => c.Id == Id);
        if (db_item is null) return;

        _Set.Remove(db_item);
    }

    public virtual async Task DeleteAsync(Guid Id)
    {
        var db_item = await _Set.FirstOrDefaultAsync(c => c.Id == Id);
        if (db_item is null) return;

        _Set.Remove(db_item);
    }

    public virtual List<T> GetAll() => ItemsNotDeleted.ToList();

    public virtual async Task<List<T>> GetAllAsync()
    {
        var items = await ItemsNotDeleted.ToListAsync();
        return items;
    }

    public virtual Task<T?> GetByHashAsync(string Hash) => throw new NotSupportedException();

    public virtual T? GetByHash(string Hash) => throw new NotSupportedException();

    public virtual Task<T?> GetByNameAsync(string Name) => throw new NotSupportedException();

    public virtual T? GetByName(string Name) => throw new NotSupportedException();

    public virtual T? GetById(Guid Id) => ItemsNotDeleted.FirstOrDefault(item => item.Id == Id);

    public virtual async Task<T?> GetByIdAsync(Guid Id)
    {
        var item = await ItemsNotDeleted.FirstOrDefaultAsync(item => item.Id == Id);
        return item;
    }

    public virtual void Update(T entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var db_entity = GetById(entity.Id);

        if (db_entity is null) throw new ArgumentNullException(nameof(db_entity));

        var entity_to_update = UpdateCurrentEntity(entity, db_entity);
        _Set.Update(entity_to_update);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var db_entity = await GetByIdAsync(entity.Id);

        if (db_entity is null) throw new ArgumentNullException(nameof(db_entity));

        var entity_to_update = UpdateCurrentEntity(entity, db_entity);
        _Set.Update(entity_to_update);
    }

    protected virtual T UpdateCurrentEntity(T DataEntity, T DbEntity) => DataEntity;

    public virtual List<T> GetPage(int PageNumber, int PageSize)
    {
        var page = ItemsNotDeleted
           .Skip((PageNumber - 1) * PageSize)
           .Take(PageSize)
           .ToList();

        return page;
    }

    public virtual async Task<List<T>> GetPageAsync(int PageNumber, int PageSize)
    {
        var page = await ItemsNotDeleted
           .Skip((PageNumber - 1) * PageSize)
           .Take(PageSize)
           .ToListAsync();

        return page;
    }
}
