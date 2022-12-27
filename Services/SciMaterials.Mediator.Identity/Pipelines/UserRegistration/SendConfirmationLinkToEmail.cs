using Mediator;

namespace SciMaterials.Mediator.Identity.Pipelines.UserRegistration;

public sealed record SendConfirmationLinkToEmail(string UserId, string AccessToken) : IRequest;

public sealed class SendConfirmationLinkToEmailHandler : IRequestHandler<SendConfirmationLinkToEmail, Unit>
{
    public async ValueTask<Unit> Handle(SendConfirmationLinkToEmail Request, CancellationToken Cancel)
    {
        return Unit.Value;
    }
}