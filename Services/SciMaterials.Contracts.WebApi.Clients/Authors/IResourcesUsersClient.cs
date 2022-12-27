namespace SciMaterials.Contracts.WebApi.Clients.Authors;

public interface IResourcesUsersClient
{
    ValueTask<Result.Result> RegisterUser(Guid UserId, string Nickname);
}
