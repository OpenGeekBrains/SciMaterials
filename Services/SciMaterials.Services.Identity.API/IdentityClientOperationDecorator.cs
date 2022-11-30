using Microsoft.Extensions.Logging;

using SciMaterials.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Services.Identity.API;

/// <summary>
/// Decorator that cover every IdentityClient method with try catch
/// to handle Cancellation and Unhandled exceptions throws
/// </summary>
public class IdentityClientOperationDecorator : IIdentityApi
{
    private readonly ILogger<IdentityClientOperationDecorator> _Logger;
    private readonly IdentityClient _IdentityApiImplementation;

    private static readonly Error __OperationCanceledError = Errors.App.OperationCanceled;
    private static readonly Error __OperationExecutionError = Errors.App.ClientUnhandled;

    public IdentityClientOperationDecorator(ILogger<IdentityClientOperationDecorator> Logger, IdentityClient IdentityApiImplementation)
    {
        _Logger                         = Logger;
        _IdentityApiImplementation = IdentityApiImplementation;
    }
    public async Task<Result<RegisterUserResponse>> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.RegisterUserAsync(RegisterRequest, cancel));
        return result;
    }

    public async Task<Result<LoginUserResponse>> LoginUserAsync(LoginRequest LoginRequest, CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.LoginUserAsync(LoginRequest, Cancel));
        return result;
    }

    public async Task<Result> LogoutUserAsync(CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.LogoutUserAsync(Cancel));
        return result;
    }

    public async Task<Result<AuthUser>> GetUserByEmailAsync(string Email, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetUserByEmailAsync(Email, CancellationToken));
        return result;
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.ChangePasswordAsync(ChangePasswordRequest, Cancel));
        return result;
    }

    public async Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserRequest, CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.EditUserNameByEmailAsync(editUserRequest, Cancel));
        return result;
    }

    public async Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetAllUsersAsync(CancellationToken));
        return result;
    }

    public async Task<Result> DeleteUserByEmailAsync(string Email, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.DeleteUserByEmailAsync(Email, CancellationToken));
        return result;
    }

    public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetRefreshTokenAsync(Cancel));
        return result;
    }

    public async Task<Result> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.CreateRoleAsync(CreateRoleRequest, Cancel));
        return result;
    }

    public async Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetAllRolesAsync(Cancel));
        return result;
    }

    public async Task<Result<AuthRole>> GetRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetRoleByIdAsync(RoleId, CancellationToken));
        return result;
    }

    public async Task<Result> EditRoleNameByIdAsync(EditRoleNameByIdRequest EditRoleRequest, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.EditRoleNameByIdAsync(EditRoleRequest, CancellationToken));
        return result;
    }

    public async Task<Result> DeleteRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.DeleteRoleByIdAsync(RoleId, CancellationToken));
        return result;
    }

    public async Task<Result> AddRoleToUserAsync(AddRoleToUserRequest AddRoleRequest, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.AddRoleToUserAsync(AddRoleRequest, CancellationToken));
        return result;
    }

    public async Task<Result> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.DeleteUserRoleByEmailAsync(Email, RoleName, CancellationToken));
        return result;
    }

    public async Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync(string Email, CancellationToken CancellationToken = default)
    {
        var result = await HandleOperation(_IdentityApiImplementation.GetUserRolesAsync(Email, CancellationToken));
        return result;
    }

    private async Task<TResult> HandleOperation<TResult>(Task<TResult> Operation) where TResult : Result, new()
    {
        try
        {
            return await Operation;
        }
        catch (OperationCanceledException)
        {
            return new TResult
            {
                Code = __OperationCanceledError.Code,
                Message = __OperationCanceledError.Message
            };
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Fail on call server");
            return new TResult
            {
                Code    = __OperationExecutionError.Code,
                Message = __OperationExecutionError.Message
            };
        }
    }

}
