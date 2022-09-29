
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Data.Repositories;
using SciMaterials.Data.Repositories.UserRepositories;

namespace SciMaterials.Data.UnitOfWork;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly ILogger _logger;
    private readonly TContext _context;

    private bool disposed;
    private Dictionary<string, object>? _repositories;

    /// <summary> ctor. </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <exception cref="ArgumentException"></exception>
    public UnitOfWork(
        ILogger logger,
        TContext context)
    {
        _logger = logger;
        _logger.Debug($"Логгер встроен в {nameof(UnitOfWork)}.");

        _context = context ?? throw new ArgumentException(nameof(context));
    }

    /// <summary> Получить экземпляр репозитория. </summary>
    /// <typeparam name="T"> Модель для которой необходимо получить репозиторий. </typeparam>
    /// <returns> Экземпляр репозитория. </returns>
    public IRepository<T> GetRepository<T>() where T : class
    {
        _logger.Debug($"{nameof(UnitOfWork)} >>> {nameof(GetRepository)}.");

        if (_repositories == null)
            _repositories = new Dictionary<string, object>();

        var type = nameof(T);

        if (!_repositories.ContainsKey(type))
        {
            switch (type)
            {
                case nameof(User):
                    _repositories.Add(type, new UserRepository(_context, _logger));
                    break;
                default:
                    _logger.Error($"Ошибка при попытке создания экземпляра репозитория для {nameof(T)}.");
                    break;
            }
        }
        return (IRepository<T>)_repositories[type];
    }

    /// <summary> Сохранить изменения контекста. </summary>
    /// <returns> Кол-во затронутых строк в БД. </returns>
    public int SaveContext()
    {
        _logger.Info($"{nameof(UnitOfWork)} >>> {nameof(SaveContext)}.");
        try
        {
            return _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.Error($"{nameof(UnitOfWork)} >>> {nameof(SaveContext)}. Ошибка при попытке сохранений изменений контекста. >>> {ex.Message}");
            return 0;
        }
    }

    /// <summary> Начать транзакцию. </summary>
    /// <returns> Транзакция. </returns>
    public IDbContextTransaction BeginTransaction(bool useIfExists = false)
    {
        var transaction = _context.Database.CurrentTransaction;
        if (transaction == null)
        {
            return _context.Database.BeginTransaction();
        }

        return useIfExists ? transaction : _context.Database.BeginTransaction();
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