using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

[Table("Urls")]
public class Url : Resource
{
    public string? Link { get; set; }
}
