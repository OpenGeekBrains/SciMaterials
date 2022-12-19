using System.ComponentModel.DataAnnotations.Schema;
using SciMaterials.DAL.Contracts.Enums;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

[Table("Urls")]
public class Url : Resource
{
    public override ResourceType ResourceType => ResourceType.Url;

    public string? Link { get; set; }
}
