using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Repositories;

public class RepositoryInclude<T> : Repository<T> where T : BaseModel
{
    public RepositoryInclude(Repository<T> BaseRepository) : base(BaseRepository.Context, BaseRepository.Logger)
    {
        Include    = true;
        NoTracking = BaseRepository.NoTracking;
    }
}