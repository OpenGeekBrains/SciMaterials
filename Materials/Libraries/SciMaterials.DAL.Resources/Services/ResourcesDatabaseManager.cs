using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Contracts.Services;
using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.DAL.Resources.Services;

public class ResourcesDatabaseManager : IDatabaseManager
{
    private readonly SciMaterialsContext _db;
    private readonly ILogger<SciMaterialsContext> _Logger;

    public ResourcesDatabaseManager(SciMaterialsContext db, ILogger<SciMaterialsContext> logger)
    {
        _db = db;
        _Logger = logger;
    }

    public async Task DeleteDatabaseAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Deleting a database...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            var result = await _db.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false);
            if (result)
                _Logger.LogInformation("Database was removed successfully");
            else
                _Logger.LogInformation("Database not removed because it not exists");
        }
        catch (OperationCanceledException e)
        {
            _Logger.LogError(e, "Interrupting an operation when deleting a database");
            throw;
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Error during database initialization");
            throw;
        }
    }

    public async Task InitializeDatabaseAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Database initialization...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            if (_db.Database.IsRelational())
            {
                var pending_migrations = (await _db.Database.GetPendingMigrationsAsync(Cancel).ConfigureAwait(false)).ToArray();
                var applied_migrations = (await _db.Database.GetAppliedMigrationsAsync(Cancel)).ToArray();

                _Logger.LogInformation("Pending migrations {0}:  {1}", pending_migrations.Length, string.Join(",", pending_migrations));
                _Logger.LogInformation("Applied migrations {0}:  {1}", pending_migrations.Length, string.Join(",", applied_migrations));

                // если есть неприменённые миграции, то их надо применить
                if (pending_migrations.Length > 0) 
                {
                    await _db.Database.MigrateAsync(Cancel);
                    _Logger.LogInformation("Migrate database successfully");
                }
                // если не было неприменённых миграций, и нет ни одной применённой миграции, то это значит, что системы миграций вообще нет для этого поставщика БД. Надо просто создать БД.
                else if (applied_migrations.Length == 0) 
                {
                    await _db.Database.EnsureCreatedAsync(Cancel);
                    _Logger.LogInformation("Migrations not supported by provider. Database created.");
                }
            }
            else
            {
                await _db.Database.EnsureCreatedAsync(Cancel);
                _Logger.LogInformation("Migrations not supported by provider. Database created.");
            }
        }
        catch (OperationCanceledException e)
        {
            _Logger.LogError(e, "Interrupting an operation when deleting a database");
            throw;
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Error during database initialization");
            throw;
        }
    }

    public async Task SeedDatabaseAsync(CancellationToken Cancel = default)
    {
        await DataSeeder.SeedAsync(_db, _Logger, Cancel).ConfigureAwait(false);
    }
}