using System.ComponentModel.DataAnnotations.Schema;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

[Table("FileGroups")]
public class FileGroup : Resource
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
