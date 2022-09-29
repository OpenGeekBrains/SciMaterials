using SciMaterials.DAL.Contexts;

namespace SciMaterials.DAL.InitializationDb.Implementation
{
    public static class DataSeeder
    {
        public static async Task Seed(SciMaterialsContext db, CancellationToken cancel = default)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancel).ConfigureAwait(false);



            await transaction.CommitAsync(cancel).ConfigureAwait(false);
        }
    }
}
