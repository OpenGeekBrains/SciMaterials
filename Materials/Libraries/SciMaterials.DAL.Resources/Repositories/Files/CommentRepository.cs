using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Files;

namespace SciMaterials.DAL.Resources.Repositories.Files;

/// <summary>Репозиторий для <see cref="Comment"/></summary>
public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(SciMaterialsContext context, ILogger<CommentRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Comment> GetIncludeQuery(IQueryable<Comment> query) => query
       .Include(c => c.Author)
       .Include(c => c.Resource);

    protected override Comment UpdateCurrentEntity(Comment DataEntity, Comment DbEntity) 
    {
        DbEntity.CreatedAt = DataEntity.CreatedAt;
        DbEntity.ResourceId = DataEntity.ResourceId;
        DbEntity.Resource = DataEntity.Resource;
        DbEntity.ParentId = DataEntity.ParentId;
        DbEntity.Text = DataEntity.Text;
        DbEntity.Author = DataEntity.Author;
        DbEntity.AuthorId = DataEntity.AuthorId;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        return DbEntity;
    }
}