using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Auth;
using SciMaterials.DAL.AUTH.Context;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class AuthDbInitializer : IAuthDbInitializer
{
    private readonly AuthDbContext _db;
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AuthDbContext> _Logger;

    public AuthDbInitializer(
        AuthDbContext DBContext, 
        UserManager<IdentityUser> UserManager, 
        RoleManager<IdentityRole> RoleManager, 
        IConfiguration Configuration,
        ILogger<AuthDbContext> Logger)
    {
        _db = DBContext;
        _UserManager = UserManager;
        _RoleManager = RoleManager;
        _Configuration = Configuration;
        _Logger = Logger;
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

    public async Task InitializeAsync(bool RemoveAtStart = false, CancellationToken Cancel = default)
    {
        _Logger.Log(LogLevel.Information,"Initialize auth database start {Time}", DateTime.Now);

        if (RemoveAtStart)
        {
            _Logger.LogInformation("Need to remove database at begging of initialization process");
            if (await DeleteDbAsync(Cancel).ConfigureAwait(false))
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

        await AuthRolesInitializer.InitializeAsync(_UserManager, _RoleManager, _Configuration);

        _Logger.Log(LogLevel.Information,"Initialize auth database stop {Time}", DateTime.Now);
    }
}