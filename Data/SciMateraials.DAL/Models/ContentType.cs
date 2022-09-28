namespace SciMaterials.DAL.Models;

public partial class ContentType : NamedModel
{
    public ICollection<File> Files { get; set; }

    public ContentType()
    {
        Files = new HashSet<File>();
    }
}
