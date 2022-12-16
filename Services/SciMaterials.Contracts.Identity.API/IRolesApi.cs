using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.API;

/// <summary> Интерфейс для ролей </summary>
public interface IRolesApi
{
    /// <summary> Метод создания роли для Identity </summary>
    /// <param name="CreateRoleRequest"> Запрос на создание роли </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè </returns>
    Task<Result.Result> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ ïîëó÷åíèÿ èíô. î âñåõ ðîëÿõ â Identity </summary>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè è ïðè óäà÷íîì ñòå÷åíèè ñïèñîê ðîëåé </returns>
    Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ ïîëó÷åíèÿ èíô. î ðîëè ïî èäåíòèôèêàòîðó â Identity </summary>
    /// <param name="RoleId"> Èäåíòèôèêàòîð ðîëè </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè è ïðè óäà÷íîì ñòå÷åíèè ðîëü èìåþùóþ óêàçàííûé <paramref name="RoleId"/> </returns>
    Task<Result<AuthRole>> GetRoleByIdAsync(string RoleId, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ ðåäàêòèðîâàíèÿ ðîëè ïî èäåíòèôèêàòîðó â Identity </summary>
    /// <param name="EditRoleRequest"> Çàïðîñ íà ðåäàêòèðîâàíèå ðîëè ïî èäåíòèôèêàòîðó </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè </returns>
    Task<Result.Result> EditRoleNameByIdAsync(EditRoleNameByIdRequest EditRoleRequest, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè íà óäàëåíèå ðîëè ïî èäåíòèôèêàòîðó â Identity </summary>
    /// <param name="RoleId"> Èäåíòèôèêàòîð ðîëè </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè </returns>
    Task<Result.Result> DeleteRoleByIdAsync(string RoleId, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ äîáàâëåíèÿ ðîëè ê ïîëüçîâàòåëþ â Identity </summary>
    /// <param name="AddRoleRequest"> Çàïðîñ íà äîáàâëåíèå ðîëè </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè </returns>
    Task<Result.Result> AddRoleToUserAsync(AddRoleToUserRequest AddRoleRequest, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ óäàëåíèÿ ðîëè ïîëüçîâàòåëÿ ïî email â Identity </summary>
    /// <param name="Email"> Email ïîëüçîâàòåëÿ </param>
    /// <param name="RoleName"> Èìÿ ðîëè </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns> Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè </returns>
    Task<Result.Result> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken Cancel = default);

    /// <summary> Ìåòîä àïè äëÿ ïîëó÷åíèÿ èíôîðìàöèè î âñåõ ðîëÿõ â ñèñòåìå â Identity </summary>
    /// <param name="Email"> Email ïîëüçîâàòåëÿ </param>
    /// <param name="Cancel"> Òîêåí îòìåíû </param>
    /// <returns>
    /// Ðåçóëüòàò âûïîëíåíèÿ îïåðàöèè è ïðè óäà÷íîì ñòå÷åíèè ñïèñîê ðîëåé
    /// îòíîñÿùèõñÿ ê óêàçàííîìó ïîëüçîâàòåëþ ñ <paramref name="Email"/>
    /// </returns>
    Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync(string Email, CancellationToken Cancel = default);
}