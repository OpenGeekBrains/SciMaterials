using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

[Table("FileGroups")]
public class FileGroup : Resource
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
