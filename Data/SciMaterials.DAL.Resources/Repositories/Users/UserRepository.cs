using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Contracts.Repositories;
using SciMaterials.DAL.Resources.Contracts.Repositories.Users;

namespace SciMaterials.DAL.Resources.Repositories.Users;

/// <summary> Репозиторий для <see cref="User"/>. </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SciMaterialsContext context, ILogger<UserRepository> Logger) : base(context, Logger) { }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByNameAsync(string, bool, bool)"/>
    public override Task<User?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
        //return await base.GetByNameAsync(name); // throw exception
    }

    ///
    /// <inheritdoc cref="IRepository{T}.GetByName(string, bool, bool)"/>
    public override User? GetByName(string name)
    {
        throw new NotImplementedException();
        //return base.GetByName(name); // throw exception
    }

    protected override User UpdateCurrentEntity(User DataEntity, User DbEntity)
    {
        DbEntity.IsDeleted = DataEntity.IsDeleted;
        return DbEntity;
    }
}