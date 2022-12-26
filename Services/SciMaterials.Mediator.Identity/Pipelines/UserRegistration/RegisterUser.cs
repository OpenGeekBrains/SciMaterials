using Mediator;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Mediator.Identity.Pipelines.UserRegistration;

public sealed record RegisterUser(string Email, string UserName, string Password) : IRequest<Result>;


public sealed class RegisterUserHandler : IRequestHandler<RegisterUser, Result<RegisterUserResponse>>
{
    public async ValueTask<Result<RegisterUserResponse>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        return null;
    }
}