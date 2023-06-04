using System.ComponentModel.DataAnnotations.Schema;
using SciMaterials.DAL.Contracts.Enums;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

[Table("Files")]
public class File : Resource
{
    public long Size { get; set; }
    public string? Hash { get; set; }
    public Guid? ContentTypeId { get; set; }
    public Guid? FileGroupId { get; set; }
    public int AntivirusScanStatus { get; set; }
    public DateTime? AntivirusScanDate { get; set; }
    public string ShortLink { get; set; } = string.Empty;
    public override ResourceType ResourceType => ResourceType.File;

    public ContentType? ContentType { get; set; }
    public FileGroup? FileGroup { get; set; }
}
