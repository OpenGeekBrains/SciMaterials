using MediatR;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Mediator.Identity.Pipelines;

public record NotifyModeratorsAboutProblem(Error Error) : INotification;

public sealed class NotifyModeratorsAboutProblemHandler : INotificationHandler<NotifyModeratorsAboutProblem>
{
    private readonly ILogger<NotifyModeratorsAboutProblemHandler> _Logger;

    public NotifyModeratorsAboutProblemHandler(ILogger<NotifyModeratorsAboutProblemHandler> Logger)
    {
        _Logger = Logger;
    }

    public async Task Handle(NotifyModeratorsAboutProblem Notification, CancellationToken Cancel)
    {
        _Logger.LogInformation(
            101,
            "Get task on work with error code: {errorCode}, and error message: {message}",
            Notification.Error.Code,
            Notification.Error.Message);
        // Build Task data for moderators to handle problem in system
    }
}