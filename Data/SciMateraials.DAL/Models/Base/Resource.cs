namespace SciMaterials.DAL.Models.Base;

public class Resource : NamedModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public User Owner { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Rating> Ratings { get; set; }

    public Resource()
    {
        Comments = new HashSet<Comment>();
        Tags = new HashSet<Tag>();
        Ratings = new HashSet<Rating>();
    }
}