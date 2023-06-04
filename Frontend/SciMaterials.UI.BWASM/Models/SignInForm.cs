using System.ComponentModel.DataAnnotations;

namespace SciMaterials.UI.BWASM.Models;

public class SignInForm
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}