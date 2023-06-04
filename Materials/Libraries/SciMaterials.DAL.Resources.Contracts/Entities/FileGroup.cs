using System.ComponentModel.DataAnnotations.Schema;
using SciMaterials.DAL.Contracts.Enums;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

[Table("FileGroups")]
public class FileGroup : Resource
{
    public override ResourceType ResourceType => ResourceType.FileGroup;

    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
