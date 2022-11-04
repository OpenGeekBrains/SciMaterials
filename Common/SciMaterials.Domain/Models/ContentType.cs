using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class ContentType : NamedModel
{
    public string FileExtension { get; set; } = null!;
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
