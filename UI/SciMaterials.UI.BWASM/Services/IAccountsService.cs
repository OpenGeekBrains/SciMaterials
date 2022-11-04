using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public interface IAccountsService
{
    Task<IReadOnlyList<UserInfo>> UsersList();
    Task ChangeAuthority(string userEmail, string authorityName);
    Task Delete(string userEmail);
}