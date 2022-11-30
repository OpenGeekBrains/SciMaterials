namespace SciMaterials.Contracts.Identity.API.Requests.Users;

public class EditUserNameByEmailRequest
{
    public string UserEmail { get; init; } = null!;
    public string EditUserNickName { get; init; } = null!;
}