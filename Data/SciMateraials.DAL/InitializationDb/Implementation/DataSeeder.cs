using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Properties;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.DAL.InitializationDb.Implementation
{
    internal static class DataSeeder
    {
        internal static async Task Seed(SciMaterialsContext db, CancellationToken cancel = default)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancel).ConfigureAwait(false);

            if (!await db.Files.AnyAsync(cancel))
            {
                try
                {
                    await db.Files.AddRangeAsync(JsonConvert.DeserializeObject<List<File>>(Encoding.UTF8.GetString(Resources.Files))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Files", e.Message);
                    throw;
                }
            }

            if (!await db.Users.AnyAsync(cancel))
            {
                try
                {
                    await db.Users.AddRangeAsync(JsonConvert.DeserializeObject<List<User>>(Encoding.UTF8.GetString(Resources.Users))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Users", e.Message);
                    throw;
                }
            }

            if (!await db.Categories.AnyAsync(cancel))
            {
                try
                {
                    await db.Categories.AddRangeAsync(JsonConvert.DeserializeObject<List<Category>>(Encoding.UTF8.GetString(Resources.Comments))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Categories", e.Message);
                    throw;
                }
            }

            if (!await db.Comments.AnyAsync(cancel))
            {
                try
                {
                    await db.Comments.AddRangeAsync(JsonConvert.DeserializeObject<List<Comment>>(Encoding.UTF8.GetString(Resources.Comments))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Comments", e.Message);
                    throw;
                }
            }

            if (!await db.FileGroups.AnyAsync(cancel))
            {
                try
                {
                    await db.FileGroups.AddRangeAsync(JsonConvert.DeserializeObject<List<FileGroup>>(Encoding.UTF8.GetString(Resources.Ratings))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data FileGroup", e.Message);
                    throw;
                }
            }

            if (!await db.Tags.AnyAsync(cancel))
            {
                try
                {
                    await db.Tags.AddRangeAsync(JsonConvert.DeserializeObject<List<Tag>>(Encoding.UTF8.GetString(Resources.Tags))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Tags", e.Message);
                    throw;
                }
            }

            if (!await db.Ratings.AnyAsync(cancel))
            {
                try
                {
                    await db.Ratings.AddRangeAsync(JsonConvert.DeserializeObject<List<Rating>>(Encoding.UTF8.GetString(Resources.Ratings))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data Ratings", e.Message);
                    throw;
                }
            }

            if (!await db.ContentTypes.AnyAsync(cancel))
            {
                try
                {
                    await db.ContentTypes.AddRangeAsync(JsonConvert.DeserializeObject<List<ContentType>>(Encoding.UTF8.GetString(Resources.ContentTypes))!, cancel)
                        .ConfigureAwait(false);
                    await db.SaveChangesAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error loading data ContentTypes", e.Message);
                    throw;
                }
            }

            await transaction.CommitAsync(cancel).ConfigureAwait(false);
        }
    }
}
