using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Link : BaseModel
{
    public string SourceAddress { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public int AccessCount { get; set; }
    public int LastAccess { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}