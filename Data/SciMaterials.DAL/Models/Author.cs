using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Author : NamedModel
{
    public string Email { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
