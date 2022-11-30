using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

public class Tag : NamedModel
{
    public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();
}
