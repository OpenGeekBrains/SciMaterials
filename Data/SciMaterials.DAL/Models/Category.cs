using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Category : NamedModel
{
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<File> Files { get; set; } = new HashSet<File>();
    public ICollection<FileGroup> FileGroups { get; set; } = new HashSet<FileGroup>();
}
