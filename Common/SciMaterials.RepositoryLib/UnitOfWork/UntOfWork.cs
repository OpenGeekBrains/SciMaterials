using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryLib.Repositories.RatingRepositories;
using SciMaterials.RepositoryLib.Repositories.UrlsRepositories;
using SciMaterials.RepositoryLib.Repositories.UsersRepositories;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Data.UnitOfWork;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly ILogger<UnitOfWork<TContext>> _logger;
    private readonly TContext _context;

    private bool disposed;
    private Dictionary<Type, object>? _repositories = new();

    /// <summary> ctor. </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <exception cref="ArgumentException"></exception>
    public UnitOfWork(
        ILogger<UnitOfWork<TContext>> logger,
        TContext context)
    {
        _logger = logger;
        _logger.LogDebug($"Логгер встроен в {nameof(UnitOfWork)}.");

        _context = context ?? throw new ArgumentException(nameof(context));

        Initialise();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.GetRepository{TEntity}"/>
    public IRepository<T> GetRepository<T>() where T : class
    {
        _logger.LogDebug($"{nameof(UnitOfWork)} >>> {nameof(GetRepository)}.");

        if (_repositories == null)
            _repositories = new Dictionary<Type, object>();

        var type = typeof(T);

        return (IRepository<T>)_repositories[type];
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContext()"/>
    public int SaveContext()
    {
        _logger.LogInformation($"{nameof(UnitOfWork)} >>> {nameof(SaveContext)}.");
        try
        {
            return _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(UnitOfWork)} >>> {nameof(SaveContext)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContextAsync()"/>
    public async Task<int> SaveContextAsync()
    {
        _logger.LogInformation($"{nameof(UnitOfWork)} >>> {nameof(SaveContextAsync)}.");
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(UnitOfWork)} >>> {nameof(SaveContextAsync)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message} >>> {ex.InnerException?.Message ?? ""}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransaction(bool)"/>
    public IDbContextTransaction BeginTransaction(bool UseIfExists = false)
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction == null)
        {
            return _context.Database.BeginTransaction();
        }

        return UseIfExists ? transaction : _context.Database.BeginTransaction();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransactionAsync(bool)"/>
    public Task<IDbContextTransaction> BeginTransactionAsync(bool UseIfExists = false)
    {
        throw new NotImplementedException();
    }

    private void Initialise()
    {
        _repositories!.Add(typeof(User), new AuthorRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(File), new FileRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Category), new CategoryRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Comment), new CommentRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(ContentType), new ContentTypeRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(FileGroup), new FileGroupRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Rating), new RatingRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Tag), new TagRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Author), new AuthorRepository((ISciMaterialsContext)_context, _logger));
        _repositories!.Add(typeof(Url), new UrlRepository((ISciMaterialsContext)_context, _logger));
    }

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            disposed = true;
        }
    }



    #endregion
}