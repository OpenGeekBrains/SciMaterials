using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class File : Resource
{
    public string? Url { get; set; }
    public long Size { get; set; }
    public Guid? ContentTypeId { get; set; }
    public Guid? FileGroupId { get; set; }

    public ContentType? ContentType { get; set; }
    public FileGroup? FileGroup { get; set; }

    public File():base(){}
}
