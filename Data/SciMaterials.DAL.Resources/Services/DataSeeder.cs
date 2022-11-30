using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.TestData;

namespace SciMaterials.DAL.Resources.Services;

public static class DataSeeder
{
    //data generation was carried out by https://generatedata.com

    /// <summary> Асинхронно выполняет транзакцию заполнения таблиц базы данных тестовыми данными. В случае ошибки транзакция не выполняется.</summary>
    /// <param name="db">Контекст базы данных.</param>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/> Значение по умолчанию: <value>default</value></param>
    /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public static async Task SeedAsync(SciMaterialsContext db, ILogger Logger, CancellationToken Cancel = default)
    {
        Logger.LogInformation("Инициализация БД тестовыми данными");

        await using var transaction = await db.Database.BeginTransactionAsync(Cancel).ConfigureAwait(false);

        if (!await db.Users.AnyAsync(Cancel))
        {
            try
            {
                var users = AssemblyResources.Users;
                await db.Users.AddRangeAsync(users!, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Users");
                throw;
            }
        }

        if (!await db.Authors.AnyAsync(Cancel))
        {
            try
            {
                var authors = AssemblyResources.Authors;
                await db.Authors.AddRangeAsync(authors!, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Authors");
                throw;
            }
        }

        if (!await db.ContentTypes.AnyAsync(Cancel))
        {
            try
            {
                await db.ContentTypes.AddRangeAsync(AssemblyResources.ContentTypes!, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data ContentTypes");
                throw;
            }
        }

        if (!await db.FileGroups.AnyAsync(Cancel))
        {
            try
            {
                await db.FileGroups.AddRangeAsync(AssemblyResources.FileGroups!, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data FileGroup");
                throw;
            }
        }

        if (!await db.Files.AnyAsync(Cancel))
        {
            try
            {
                await db.Files.AddRangeAsync(AssemblyResources.Files, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Files");
                throw;
            }
        }

        if (!await db.Urls.AnyAsync(Cancel))
        {
            try
            {
                await db.Urls.AddRangeAsync(AssemblyResources.Urls, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Urls");
                throw;
            }
        }

        if (!await db.Links.AnyAsync(Cancel))
        {
            try
            {
                var links = AssemblyResources.Links;
                //var i     = 0;
                //foreach (var link in links)
                //    link.RowVersion = BitConverter.GetBytes(i++);

                await db.Links.AddRangeAsync(AssemblyResources.Links, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Links");
                throw;
            }
        }

        if (!await db.Tags.AnyAsync(Cancel))
        {
            try
            {
                await db.Tags.AddRangeAsync(AssemblyResources.Tags, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Tags");
                throw;
            }
        }

        if (!await db.Categories.AnyAsync(Cancel))
        {
            try
            {
                await db.Categories.AddRangeAsync(AssemblyResources.Categories, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Categories");
                throw;
            }
        }

        if (!await db.Comments.AnyAsync(Cancel))
        {
            try
            {
                await db.Comments.AddRangeAsync(AssemblyResources.Comments, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Comments");
                throw;
            }
        }

        if (!await db.Ratings.AnyAsync(Cancel))
        {
            try
            {
                await db.Ratings.AddRangeAsync(AssemblyResources.Ratings, Cancel);
                await db.SaveChangesAsync(Cancel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error loading data Ratings");
                throw;
            }
        }

        await transaction.CommitAsync(Cancel);
    }
}
