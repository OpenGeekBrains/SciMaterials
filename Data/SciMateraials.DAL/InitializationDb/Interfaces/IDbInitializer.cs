namespace SciMaterials.DAL.InitializationDb.Interfaces
{
    public interface IDbInitializer
    {
        Task<bool> DeleteDbAsync(CancellationToken cancel = default);
        Task InitializeDbAsync(bool removeAtStart = false, CancellationToken cancel = default);
    }
}