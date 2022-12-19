using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.API;

/// <summary> Контракт апи ролей пользователей </summary>
public interface IRolesApi
{
    /// <summary> Метод апи для создания роли пользователя в Identity </summary>
    /// <param name="CreateRoleRequest"> Запрос на создание роли </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default);

    /// <summary> Метод апи для получения инф. о всех ролях в Identity </summary>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении список ролей </returns>
    Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default);

    /// <summary> Метод апи для получения инф. о роли по идентификатору в Identity </summary>
    /// <param name="RoleId"> Идентификатор роли </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции и при удачном стечении роль имеющую указанный <paramref name="RoleId"/> </returns>
    Task<Result<AuthRole>> GetRoleByIdAsync(string RoleId, CancellationToken Cancel = default);

    /// <summary> Метод апи для редактирования роли по идентификатору в Identity </summary>
    /// <param name="EditRoleRequest"> Запрос на редактирование роли по идентификатору </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> EditRoleNameByIdAsync(EditRoleNameByIdRequest EditRoleRequest, CancellationToken Cancel = default);

    /// <summary> Метод апи на удаление роли по идентификатору в Identity </summary>
    /// <param name="RoleId"> Идентификатор роли </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> DeleteRoleByIdAsync(string RoleId, CancellationToken Cancel = default);

    /// <summary> Метод апи для добавления роли к пользователю в Identity </summary>
    /// <param name="AddRoleRequest"> Запрос на добавление роли </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> AddRoleToUserAsync(AddRoleToUserRequest AddRoleRequest, CancellationToken Cancel = default);

    /// <summary> Метод апи для удаления роли пользователя по email в Identity </summary>
    /// <param name="Email"> Email пользователя </param>
    /// <param name="RoleName"> Имя роли </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns> Результат выполнения операции </returns>
    Task<Result.Result> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken Cancel = default);

    /// <summary> Метод апи для получения информации о всех ролях в системе в Identity </summary>
    /// <param name="Email"> Email пользователя </param>
    /// <param name="Cancel"> Токен отмены </param>
    /// <returns>
    /// Результат выполнения операции и при удачном стечении список ролей
    /// относящихся к указанному пользователю с <paramref name="Email"/>
    /// </returns>
    Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync(string Email, CancellationToken Cancel = default);
}