using System.Text;
using Newtonsoft.Json;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.Properties;

namespace SciMaterials.DAL.InitializationDb.Implementation
{
    public static class DataSeeder
    {
        public static async Task Seed(SciMaterialsContext db, CancellationToken cancel = default)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancel).ConfigureAwait(false);

            if (!db.Files.Any())
            {
                await db.Files.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Users.Any())
            {
                await db.Users.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Categories.Any())
            {
                await db.Categories.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Comments.Any())
            {
                await db.Comments.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.FileGroups.Any())
            {
                await db.FileGroups.AddRangeAsync(JsonConvert.DeserializeObject<List<FileGroup>>(Encoding.UTF8.GetString(Resources.Ratings))!, cancel).ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Tags.Any())
            {
                await db.Tags.AddRangeAsync(JsonConvert.DeserializeObject<List<Tag>>(Encoding.UTF8.GetString(Resources.Tags))!, cancel)
                    .ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Ratings.Any())
            {
                await db.Ratings.AddRangeAsync(JsonConvert.DeserializeObject<List<Rating>>(Encoding.UTF8.GetString(Resources.Ratings))!, cancel)
                    .ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.ContentTypes.Any())
            {
                await db.ContentTypes.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancel).ConfigureAwait(false);
        }
    }
}
