using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Tag : NamedModel
{
    public ICollection<File> Files { get; set; }

    public Tag()
    {
        Files = new HashSet<File>();
    }
}
