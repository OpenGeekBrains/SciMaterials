using SciMaterials.Contracts.Identity.API.DTO.Users;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;

namespace SciMaterials.Contracts.Identity.Clients.Clients;

public interface IUserClient
{
    Task<ClientCreateUserResponse> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken cancel = default);
    
    Task<ClientLoginResponse> LoginUserAsync(LoginRequest LoginRequest, CancellationToken Cancel = default);
    
    Task<ClientLogoutResponse> LogoutUserAsync(CancellationToken Cancel = default);
    
    Task<ClientCreateUserResponse> CreateUserAsync(RegisterRequest CreateRequest, CancellationToken CancellationToken = default);
    
    Task<ClientGetUserByEmailResponse> GetUserByEmailAsync(string Email, CancellationToken CancellationToken = default);
    
    Task<ClientChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default);
    
    Task<ClientEditUserNameByEmailResponse> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserRequest, CancellationToken Cancel = default);
    
    Task<ClientGetAllUsersResponse> GetAllUsersAsync(CancellationToken CancellationToken = default);

    Task<ClientDeleteUserByEmailResponse> DeleteUserByEmailAsync(string Email, CancellationToken CancellationToken = default);
    
    Task<ClientDeleteUsersWithOutConfirmResponse> DeleteUsersWithOutConfirmAsync(CancellationToken CancellationToken = default);

    Task<ClientRefreshTokenResponse> GetRefreshTokenAsync(CancellationToken Cancel = default);
}