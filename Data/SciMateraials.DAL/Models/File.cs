using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class File : NamedModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Size { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ContentTypeId { get; set; }
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;
    public ContentType? ContentType { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Rating> Ratings { get; set; }

    public File()
    {
        Comments = new HashSet<Comment>();
        Tags = new HashSet<Tag>();
        Ratings = new HashSet<Rating>();
    }
}
