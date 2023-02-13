using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.WebApi.Clients.Authors;

public class ResourcesUsersClient : IResourcesUsersClient
{
    private readonly HttpClient _Client;
    public ResourcesUsersClient(HttpClient Client) { _Client = Client; }

    public async ValueTask<Result> RegisterUser(Guid UserId, string Nickname) => Result.Success();
}
