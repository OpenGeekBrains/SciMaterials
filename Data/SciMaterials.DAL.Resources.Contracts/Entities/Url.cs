using System.ComponentModel.DataAnnotations.Schema;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

[Table("Urls")]
public class Url : Resource
{
    public string? Link { get; set; }
}
