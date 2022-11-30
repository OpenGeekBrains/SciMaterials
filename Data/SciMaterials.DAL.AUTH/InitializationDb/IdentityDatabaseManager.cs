using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.Contracts.Services;

namespace SciMaterials.DAL.AUTH.InitializationDb;

public class IdentityDatabaseManager : IDatabaseManager
{
    private readonly AuthDbContext _db;
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AuthDbContext> _Logger;

    public IdentityDatabaseManager(
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
        _Logger.Log(LogLevel.Information,"Initialize auth database start {Time}", DateTime.Now);

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
            // если не было неприменённых миграций, и нет ни одной применённой миграции, то это значит, что системы миграций вообще нет для этого поставщика БД.Надо просто создать БД.
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

        await SeedDatabaseAsync(Cancel);

        _Logger.Log(LogLevel.Information,"Initialize auth database stop {Time}", DateTime.Now);
    }

    public async Task SeedDatabaseAsync(CancellationToken Cancel = default)
    {
        await InitializeRolesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Инициализация базы данных с созданием ролей "супер админ" и "пользователь"
    /// Создание одной учетной записи "админа"
    /// </summary>
    private async Task InitializeRolesAsync()
    {
        await CheckRoleAsync("admin").ConfigureAwait(false);
        await CheckRoleAsync("user");

        var admin_settings = _Configuration.GetSection("IdentitySettings:AdminSettings");
        var admin_email = admin_settings["login"];
        var admin_password = admin_settings["password"];

        //Супер админ
        if (await _UserManager.FindByNameAsync(admin_email) is null)
        {
            var super_admin = new IdentityUser
            {
                Email = admin_email,
                UserName = admin_email
            };

            if (await _UserManager.CreateAsync(super_admin, admin_password) is { Succeeded: false, Errors: var errors })
                throw new InvalidOperationException($"Ошибка создания администратора {string.Join(",", errors.Select(e => e.Description))}");

            await _UserManager.AddToRoleAsync(super_admin, "admin");
            var token_for_admin = await _UserManager.GenerateEmailConfirmationTokenAsync(super_admin);
            await _UserManager.ConfirmEmailAsync(super_admin, token_for_admin);
        }
    }

    private async Task CheckRoleAsync(string RoleName)
    {
        if (await _RoleManager.FindByNameAsync(RoleName) is null &&
            await _RoleManager.CreateAsync(new(RoleName)) is { Succeeded: false, Errors: var errors })
            throw new InvalidOperationException(
                $"Ошибка создания роли {RoleName}: {string.Join(",", errors.Select(e => e.Description))}");
    }
}