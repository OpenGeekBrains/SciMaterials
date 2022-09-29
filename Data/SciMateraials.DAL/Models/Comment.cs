using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Comment : BaseModel
{
    public Guid? ParentId { get; set; }
    public Guid OwnerId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;

    public ICollection<File> Files { get; set; }
    public ICollection<FileGroup> FileGroups { get; set; }

    public Comment()
    {
        Files = new HashSet<File>();
        FileGroups = new HashSet<FileGroup>();
    }
}
