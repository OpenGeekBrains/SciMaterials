using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

using Url = SciMaterials.DAL.Resources.Contracts.Entities.Url;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="Url"/>. </summary>
public class ResourceRepository : Repository<Resource>, IResourceRepository
{
    public ResourceRepository(SciMaterialsContext context, ILogger<ResourceRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Resource> GetIncludeQuery(IQueryable<Resource> query) => query
       .Include(f => f.Categories)
       .Include(f => f.Author)
       .Include(f => f.Comments)
       .Include(f => f.Tags)
       .Include(f => f.Ratings);
}