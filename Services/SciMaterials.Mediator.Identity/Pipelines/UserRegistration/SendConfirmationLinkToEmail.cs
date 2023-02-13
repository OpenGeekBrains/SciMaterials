using MediatR;

namespace SciMaterials.Mediator.Identity.Pipelines.UserRegistration;

public sealed record SendConfirmationLinkToEmail(string UserId, string AccessToken) : IRequest;

public sealed class SendConfirmationLinkToEmailHandler : AsyncRequestHandler<SendConfirmationLinkToEmail>
{
    protected override async Task Handle(SendConfirmationLinkToEmail Request, CancellationToken Cancel)
    {
       
    }
}