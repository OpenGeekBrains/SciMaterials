using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories;

namespace SciMaterials.DAL.Resources.UnitOfWork;

public class SciMaterialsFilesUnitOfWork : IUnitOfWork<SciMaterialsContext>
{
    private readonly ILogger<SciMaterialsFilesUnitOfWork> _Logger;
    private readonly SciMaterialsContext _db;
    private readonly IServiceProvider _Services;

    private bool _Disposed;

    public SciMaterialsFilesUnitOfWork(
        SciMaterialsContext context,
        IServiceProvider Services,
        ILogger<SciMaterialsFilesUnitOfWork> logger)
    {
        _Logger = logger;

        _db  = context ?? throw new ArgumentException(nameof(context));
        _Services = Services;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        _Logger.LogDebug($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(GetRepository)}.");

        return _Services.GetRequiredService<IRepository<T>>();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContext()"/>
    public int SaveContext()
    {
        _Logger.LogInformation($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContext)}.");
        try
        {
            return _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _Logger.LogError(
                $"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContext)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.SaveContextAsync()"/>
    public async Task<int> SaveContextAsync()
    {
        _Logger.LogInformation($"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContextAsync)}.");
        try
        {
            return await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _Logger.LogError(
                $"{nameof(SciMaterialsFilesUnitOfWork)} >>> {nameof(SaveContextAsync)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message} >>> {ex.InnerException?.Message ?? ""}");
            return 0;
        }
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransaction(bool)"/>
    public IDbContextTransaction BeginTransaction(bool UseIfExists = false)
    {
        var transaction = _db.Database.CurrentTransaction;
        if (transaction == null)
        {
            return _db.Database.BeginTransaction();
        }

        return UseIfExists ? transaction : _db.Database.BeginTransaction();
    }

    ///
    /// <inheritdoc cref="IUnitOfWork{T}.BeginTransactionAsync(bool)"/>
    public async Task<IDbContextTransaction> BeginTransactionAsync(bool UseIfExists = false)
    {
        var transaction = _db.Database.CurrentTransaction;
        if (transaction == null)
        {
            return await _db.Database.BeginTransactionAsync();
        }

        return UseIfExists ? transaction : await _db.Database.BeginTransactionAsync();
    }

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                //_context?.Dispose(); // Не вы его создали, не вам его и уничтожать!
            }

            _Disposed = true;
        }
    }

    #endregion
}
