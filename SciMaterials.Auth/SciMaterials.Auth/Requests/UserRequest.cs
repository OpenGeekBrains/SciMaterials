namespace SciMaterials.Auth.Requests;

public class UserRequest
{
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}