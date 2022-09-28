namespace SciMaterials.DAL.Models;

public partial class Tag : NamedModel
{
    public virtual ICollection<File> Files { get; set; }

    public Tag()
    {
        Files = new HashSet<File>();
    }
}
