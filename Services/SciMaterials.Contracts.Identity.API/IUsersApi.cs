using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.API;

/// <summary> Контракт апи пользователей </summary>
public interface IUsersApi
{
    /// <summary> Метод апи для регистрации пользователя в Identity </summary>
    /// <param name="RegisterRequest"> Запрос на регистрацию </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result<RegisterUserResponse>> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken Cancel = default);

    /// <summary> Метод апи для авторизации пользователя в Identity </summary>
    /// <param name="LoginRequest"> Запрос на авторизацию </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении токен сессии </returns>
    Task<Result<LoginUserResponse>> LoginUserAsync(LoginRequest LoginRequest, CancellationToken Cancel = default);

    /// <summary> Метод апи для выхода пользователя из системы Identity </summary>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> LogoutUserAsync(CancellationToken Cancel = default);

    /// <summary> Метод клиента для получения информации о пользователе по email в Identity </summary>
    /// <param name="Email"> Email пользователя </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении данные Identity о пользователе </returns>
    Task<Result<AuthUser>> GetUserByEmailAsync(string Email, CancellationToken Cancel = default);

    /// <summary> Метод апи для смены пароля в Identity </summary>
    /// <param name="ChangePasswordRequest"> Запрос на смену пароля </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default);

    /// <summary> Метод клиента для изменения имени (ник нейма) пользователя в Identity </summary>
    /// <param name="editUserRequest"> Запрос на изменение имени </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении обновлённый токен сессии </returns>
    Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserRequest, CancellationToken Cancel = default);

    /// <summary> Метод клиента для получения информации о всех пользователях в Identity </summary>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении список пользователей записанных в системе Identity </returns>
    Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken Cancel = default);

    /// <summary> Метод клиента для удаления пользователя по email в Identity </summary>
    /// <param name="Email"> Email пользователя </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> DeleteUserByEmailAsync(string Email, CancellationToken Cancel = default);

    /// <summary>  Метод клиента для получения Refresh токена из системы Identity для текущего авторизованного пользователя </summary>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении обновлённый Refresh токен </returns>
    Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default);
}