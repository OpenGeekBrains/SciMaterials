using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.DAL.Resources.Contracts.Entities;

public class Link : BaseModel
{
    public string SourceAddress { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public int AccessCount { get; set; }
    public DateTime? LastAccess { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}