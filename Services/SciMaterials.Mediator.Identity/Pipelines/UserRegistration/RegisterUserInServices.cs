using Mediator;
using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.Mediator.Identity.Pipelines.UserRegistration;

public sealed record RegisterUserInServices(string UserId, string UserName, string Email) : IRequest<Result>;

public sealed class RegisterUserInServicesHandler : IRequestHandler<RegisterUserInServices, Result>
{
    private readonly IResourcesUsersClient _ResourcesUsersClient;

    public RegisterUserInServicesHandler(IResourcesUsersClient ResourcesUsersClient)
    {
        _ResourcesUsersClient = ResourcesUsersClient;
    }

    public async ValueTask<Result> Handle(RegisterUserInServices Request, CancellationToken Cancel)
    {
        if (!Guid.TryParse(Request.UserId, out var user_id))
        {
            return Result.Failure(Errors.Api.ResourcesUsers.Add);
        }

        Cancel.ThrowIfCancellationRequested();

        var result = await _ResourcesUsersClient.RegisterUser(user_id, Request.UserName);

        return result;
    }
}