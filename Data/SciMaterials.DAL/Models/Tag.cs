using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Tag : NamedModel
{
    public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();
}
