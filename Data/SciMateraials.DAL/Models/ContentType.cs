using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class ContentType : NamedModel
{
    public ICollection<File> Files { get; set; }

    public ContentType()
    {
        Files = new HashSet<File>();
    }
}
