using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Author : NamedModel
{
    public string Email { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = null!;
    public string Surname { get; set; } = null!;

    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<File> Files { get; set; } = new HashSet<File>();
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
