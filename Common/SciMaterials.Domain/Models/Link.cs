using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Link : BaseModel
{
    public string SourceAddress { get; set; } = null!;
    public string? Hash { get; set; }
    public Guid AuthorId { get; set; }
    public string? Description { get; set; }

    public Author Author { get; set; } = null!;
}

