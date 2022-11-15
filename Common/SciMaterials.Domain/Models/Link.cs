using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Link : BaseModel
{
    public string SourceAddress { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public int AccessCount { get; set; }
    public int LastAccess { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}

