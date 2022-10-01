
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.UnitOfWork;

/// <summary> Интерфейс UnitOfWork. </summary>
/// <typeparam name="TContext"> Контекст. </typeparam>
public interface IUnitOfWork<TContext> where TContext : DbContext
{
    /// <summary> Получить репозиторий для модели. </summary>
    /// <typeparam name="TEntity"> Модель получаемого репозитория. </typeparam>
    /// <returns> Репозиторий. </returns>
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    void Dispose();

    void Dispose(bool disposing);

    /// <summary> Сохранить изменения контекста в БД. </summary>
    /// <returns> Кол-во затронутых строк в БД. </returns>
    int SaveContext();

    /// <summary> Сохранить изменения контекста в БД. Асинхронный. </summary>
    /// <returns> Кол-во затронутых строк в БД. </returns>
    Task<int> SaveContextAsync();

    /// <summary> Начать транзакцию. </summary>
    /// <param name="useIfExists"></param>
    /// <returns></returns>
    IDbContextTransaction BeginTransaction(bool useIfExists = false);

    /// <summary> Начать транзакцию. Асинхронный. </summary>
    /// <param name="useIfExists"></param>
    /// <returns></returns>
    Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false);
}