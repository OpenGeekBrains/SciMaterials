using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary> Репозиторий для <see cref="File"/>. </summary>
public class FileRepository : Repository<File>, IFileRepository
{
    public FileRepository(SciMaterialsContext context, ILogger<FileRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<File> GetIncludeQuery(IQueryable<File> query) => query
       .Include(f => f.ContentType)
       .Include(f => f.FileGroup)
       .Include(f => f.Categories)
       .Include(f => f.Author)
       .Include(f => f.Comments)
       .Include(f => f.Tags)
       .Include(f => f.Ratings);

    public override async Task<File?> GetByHashAsync(string hash)
    {
        var items = await ItemsNotDeleted.FirstOrDefaultAsync(c => c.Hash == hash);
        return items;
    }

    public override File? GetByHash(string hash)
    {
        var items = ItemsNotDeleted.FirstOrDefault(c => c.Hash == hash);
        return items;
    }
}