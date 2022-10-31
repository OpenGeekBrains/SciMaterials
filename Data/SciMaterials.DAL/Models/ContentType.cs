using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class ContentType : NamedModel
{
    public string FileExtension { get; set; } = null!;
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
