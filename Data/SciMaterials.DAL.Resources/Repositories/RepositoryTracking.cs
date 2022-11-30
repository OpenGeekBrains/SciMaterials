using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Repositories;

public class RepositoryTracking<T> : Repository<T> where T : BaseModel
{
    public RepositoryTracking(Repository<T> BaseRepository) : base(BaseRepository.Context, BaseRepository.Logger)
    {
        Include    = BaseRepository.Include;
        NoTracking = false;
    }
}
