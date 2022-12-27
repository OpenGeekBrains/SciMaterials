using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Mediator.Identity.Pipelines.UserRegistration;

public sealed record RegisterUser(string Email, string UserName, string Password) : IRequest<Result>;


public sealed class RegisterUserHandler : IRequestHandler<RegisterUser, Result>
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly ILogger<RegisterUserHandler> _Logger;
    private readonly IMediator _Mediator;

    public RegisterUserHandler(UserManager<IdentityUser> UserManager, ILogger<RegisterUserHandler> Logger, IMediator Mediator)
    {
        _UserManager   = UserManager; 
        _Logger        = Logger;
        _Mediator = Mediator;
    }

    public async ValueTask<Result> Handle(RegisterUser Request, CancellationToken Cancel)
    {
        try
        {
            Cancel.ThrowIfCancellationRequested();

            var identity_user   = new IdentityUser { Email = Request.Email, UserName = Request.UserName };
            var identity_result = await _UserManager.CreateAsync(identity_user, Request.Password);
            
            if (!identity_result.Succeeded)
            {
                var errors = identity_result.Errors.Select(e => e.Description).ToArray();
                _Logger.LogWarning(
                    "Не удалось зарегистрировать пользователя {Email}: {errors}",
                    Request.Email,
                    string.Join(",", errors));

                return Result.Failure(Errors.Identity.Register.Fail);
            }

            await _UserManager.AddToRoleAsync(identity_user, AuthApiRoles.User);
            var email_confirm_token = await _UserManager.GenerateEmailConfirmationTokenAsync(identity_user);

            var result = await _Mediator.Send(new RegisterUserInServices(identity_user.Id, Request.UserName, Request.Email), Cancel);
            if (result.IsFaulted)
            {
                _Logger.LogWarning(
                    "Не удалось зарегистрировать пользователя {Email} в сервисах: {ErrorCode}",
                    Request.Email,
                    result.Code);
                var error = Errors.Identity.Register.FailToRegisterInServices;
                // Send notification to moderators to handle failure on registration
                await _Mediator.Publish(new NotifyModeratorsAboutProblem(error), Cancel);
                return Result.Failure(error);
            }

            await _Mediator.Send(new SendConfirmationLinkToEmail(identity_user.Id, email_confirm_token), Cancel);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Errors.App.OperationCanceled);
        }
        catch (Exception ex)
        {
            _Logger.LogError("Пользователя не удалось зарегистрировать {Ex}", ex);
            return Result.Failure(Errors.Identity.Register.Unhandled);
        }
    }
}