namespace SciMaterials.DAL.Resources.Contracts.Repositories;

public interface IDbRepository<T> : IRepository<T> where T : class
{
    public bool NoTracking { get; set; }

    public bool Include { get; set; }
}
