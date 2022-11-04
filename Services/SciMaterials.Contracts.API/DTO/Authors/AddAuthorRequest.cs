namespace SciMaterials.Contracts.API.DTO.Authors;

public class AddAuthorRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = null!;
    public string Surname { get; set; } = null!;
}