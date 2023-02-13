namespace SciMaterials.Contracts.Identity.API.Requests.Users;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
}