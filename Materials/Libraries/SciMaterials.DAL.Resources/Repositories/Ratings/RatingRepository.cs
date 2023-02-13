using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Ratings;

namespace SciMaterials.DAL.Resources.Repositories.Ratings;

/// <summary> Репозиторий для <see cref="Rating"/>. </summary>
public class RatingRepository : Repository<Rating>, IRatingRepository
{
    public RatingRepository(SciMaterialsContext context, ILogger<RatingRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Rating> GetIncludeQuery(IQueryable<Rating> query) => query
       .Include(r => r.Resource)
       .Include(r => r.User);

    protected override Rating UpdateCurrentEntity(Rating DataEntity, Rating DbEntity)
    {
        DbEntity.ResourceId  = DataEntity.ResourceId;
        DbEntity.AuthorId    = DataEntity.AuthorId;
        DbEntity.RatingValue = DataEntity.RatingValue;
        DbEntity.Resource    = DataEntity.Resource;
        DbEntity.User        = DataEntity.User;
        DbEntity.IsDeleted   = DataEntity.IsDeleted;

        return DbEntity;
    }
}