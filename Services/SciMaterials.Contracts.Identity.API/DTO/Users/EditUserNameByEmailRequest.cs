namespace SciMaterials.Contracts.API.DTO.AuthUsers;

public class EditUserNameByEmailRequest
{
    public string? UserEmail { get; set; }
    public string? EditUserNickName { get; set; }
}