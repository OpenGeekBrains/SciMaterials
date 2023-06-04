using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

public class ContentType : NamedModel
{
    public string FileExtension { get; set; } = null!;
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
