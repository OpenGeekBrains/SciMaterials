namespace SciMaterials.DAL.Models;

public partial class Comment : BaseModel
{
    public Guid? ParentId { get; set; }
    public Guid OwnerId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;

    public ICollection<File> Files { get; set; }

    public Comment()
    {
        Files = new HashSet<File>();
    }
}
