using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Comment : BaseModel
{
    public Guid? ParentId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }

    public Author Author { get; set; } = null!;
    public File? File { get; set; } = null!;
    public FileGroup? FileGroup { get; set; } = null!;
}
