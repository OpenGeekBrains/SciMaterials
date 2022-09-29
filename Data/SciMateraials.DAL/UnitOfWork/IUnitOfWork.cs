
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SciMaterials.Data.Repositories;

namespace SciMaterials.DAL.UnitOfWork;

public interface IUnitOfWork<TContext> where TContext : DbContext
{
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    void Dispose();

    void Dispose(bool disposing);

    int SaveContext();

    IDbContextTransaction BeginTransaction(bool useIfExists = false);
}