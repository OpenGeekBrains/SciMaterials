using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories.Users;

namespace SciMaterials.DAL.Resources.Repositories.Users;

/// <summary> Репозиторий для <see cref="Author"/>. </summary>
public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(SciMaterialsContext context, ILogger<AuthorRepository> Logger) : base(context, Logger) { }

    protected override IQueryable<Author> GetIncludeQuery(IQueryable<Author> query) => query
       .Include(u => u.Comments)
       .Include(u => u.Resources)
       .Include(u => u.Ratings)
       .Include(u => u.User);

    public override Author? GetByName(string Name)
    {
        var author = ItemsNotDeleted.FirstOrDefault(item => item.Name == Name);
        return author;
    }

    public override async Task<Author?> GetByNameAsync(string Name)
    {
        var author = await ItemsNotDeleted.FirstOrDefaultAsync(item => item.Name == Name);
        return author;
    }

    protected override Author UpdateCurrentEntity(Author DataEntity, Author DbEntity)
    {
        DbEntity.Name = DataEntity.Name;
        DbEntity.IsDeleted = DataEntity.IsDeleted;

        DbEntity.Email = DataEntity.Email;
        DbEntity.UserId = DataEntity.UserId;
        DbEntity.Phone = DataEntity.Phone;
        DbEntity.Surname = DataEntity.Surname;
        DbEntity.User = DataEntity.User;
        DbEntity.Comments = DataEntity.Comments;
        DbEntity.Resources = DataEntity.Resources;
        DbEntity.Ratings = DataEntity.Ratings;

        return DbEntity;
    }
}