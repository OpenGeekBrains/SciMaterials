namespace SciMaterials.DAL.Models;

public partial class File : NamedModel
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

    public File()
    {
        Comments = new HashSet<Comment>();
        Tags = new HashSet<Tag>();
    }
}
