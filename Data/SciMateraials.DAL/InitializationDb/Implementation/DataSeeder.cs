using SciMaterials.DAL.Contexts;

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
                await db.FileGroups.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Tags.Any())
            {
                await db.Tags.AddRangeAsync().ConfigureAwait(false);
                await db.SaveChangesAsync(cancel).ConfigureAwait(false);
            }

            if (!db.Ratings.Any())
            {
                await db.Ratings.AddRangeAsync().ConfigureAwait(false);
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
