using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Database.Initialization;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Services;

namespace SciMaterials.Services.Database.Services.DbInitialization;

public class DbInitializer : IDbInitializer
{
    private readonly SciMaterialsContext _db;
    private readonly ILogger<SciMaterialsContext> _Logger;

    public DbInitializer(SciMaterialsContext db, ILogger<SciMaterialsContext> logger)
    {
        _db = db;
        _Logger = logger;
    }

    public async Task<bool> DeleteDbAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Deleting a database...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            var result = await _db.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false);
            return result;
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

    public async Task InitializeDbAsync(bool RemoveAtStart = false, bool UseDataSeeder = false, CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Database initialization...");

        Cancel.ThrowIfCancellationRequested();

        try
        {
            if (RemoveAtStart)
            {
                _Logger.LogInformation("Need to remove database at begging of initialization process");
                if(await DeleteDbAsync(Cancel).ConfigureAwait(false))
                    _Logger.LogInformation("Database was removed successfully");
                else
                    _Logger.LogInformation("Database not removed because it not exists");
            }

            if (_db.Database.IsRelational())
            {
                var pending_migrations = (await _db.Database.GetPendingMigrationsAsync(Cancel).ConfigureAwait(false)).ToArray();
                var applied_migrations = (await _db.Database.GetAppliedMigrationsAsync(Cancel)).ToArray();

                _Logger.LogInformation("Pending migrations {0}:  {1}", pending_migrations.Length, string.Join(",", pending_migrations));
                _Logger.LogInformation("Applied migrations {0}:  {1}", pending_migrations.Length, string.Join(",", applied_migrations));

                if (pending_migrations.Length > 0) // если есть неприменённые миграции, то их надо применить
                {
                    await _db.Database.MigrateAsync(Cancel);
                    _Logger.LogInformation("Migrate database successfully");
                }
                else if
                    (applied_migrations.Length == 0) // если не было неприменённых миграций, и нет ни одной применённой миграции, то это значит, что системы миграций вообще нет для этого поставщика БД. Надо просто создать БД.
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


            if (UseDataSeeder)
                await InitializeDbAsync(Cancel).ConfigureAwait(false);
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

    private async Task InitializeDbAsync(CancellationToken Cancel = default)
    {
        await DataSeeder.SeedAsync(_db, _Logger, Cancel).ConfigureAwait(false);
    }
}