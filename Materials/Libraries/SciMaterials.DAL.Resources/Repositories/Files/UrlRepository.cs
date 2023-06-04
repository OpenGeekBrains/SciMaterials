using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

using Url = SciMaterials.DAL.Resources.Contracts.Entities.Url;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="Url"/>. </summary>
public class UrlRepository : Repository<Url>, IUrlRepository
{
    public UrlRepository(SciMaterialsContext context, ILogger<UrlRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Url> GetIncludeQuery(IQueryable<Url> query) => query
       .Include(f => f.Categories)
       .Include(f => f.Author)
       .Include(f => f.Comments)
       .Include(f => f.Tags)
       .Include(f => f.Ratings);
}