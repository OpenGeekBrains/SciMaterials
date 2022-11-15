using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Properties;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.Services;

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
                var users = JsonConvert.DeserializeObject<List<User>>(Encoding.UTF8.GetString(Resources.Users));
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
                await db.Authors.AddRangeAsync(JsonConvert.DeserializeObject<List<Author>>(Encoding.UTF8.GetString(Resources.Authors))!, Cancel);
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
                await db.ContentTypes.AddRangeAsync(JsonConvert.DeserializeObject<List<ContentType>>(Encoding.UTF8.GetString(Resources.ContentTypes))!, Cancel);
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
                await db.FileGroups.AddRangeAsync(JsonConvert.DeserializeObject<List<FileGroup>>(Encoding.UTF8.GetString(Resources.FileGroups))!, Cancel);
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
                await db.Files.AddRangeAsync(JsonConvert.DeserializeObject<List<File>>(Encoding.UTF8.GetString(Resources.Files))!, Cancel);
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
                await db.Urls.AddRangeAsync(JsonConvert.DeserializeObject<List<Url>>(Encoding.UTF8.GetString(Resources.Urls))!, Cancel);
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
                await db.Links.AddRangeAsync(JsonConvert.DeserializeObject<List<Link>>(Encoding.UTF8.GetString(Resources.Links))!, Cancel);
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
                await db.Tags.AddRangeAsync(JsonConvert.DeserializeObject<List<Tag>>(Encoding.UTF8.GetString(Resources.Tags))!, Cancel);
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
                await db.Categories.AddRangeAsync(JsonConvert.DeserializeObject<List<Category>>(Encoding.UTF8.GetString(Resources.Categories))!, Cancel);
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
                await db.Comments.AddRangeAsync(JsonConvert.DeserializeObject<List<Comment>>(Encoding.UTF8.GetString(Resources.Comments))!, Cancel);
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
                await db.Ratings.AddRangeAsync(JsonConvert.DeserializeObject<List<Rating>>(Encoding.UTF8.GetString(Resources.Ratings))!, Cancel);
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
