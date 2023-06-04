using SciMaterials.DAL.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories;
using SciMaterials.DAL.Resources.Repositories;

namespace SciMaterials.DAL.Resources.Extensions;

public static class DbRepositoryEx
{
    public static IRepository<T> Include<T>(this IRepository<T> repository) where T : BaseModel
    {
        if (repository is not Repository<T> { Include: false } db_repository)
            return repository;

        var repository_included = new RepositoryInclude<T>(db_repository);

        return repository_included;
    }

    public static IRepository<T> Tracking<T>(this IRepository<T> repository) where T : BaseModel
    {
        if (repository is not Repository<T> { NoTracking: true } db_repository)
            return repository;

        var repository_included = new RepositoryTracking<T>(db_repository);

        return repository_included;
    }
}
