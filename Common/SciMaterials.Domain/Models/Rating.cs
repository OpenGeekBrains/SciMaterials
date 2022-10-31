using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Rating : BaseModel
{
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }
    public Guid AuthorId { get; set; }
    public int RatingValue { get; set; }

    public File? File { get; set; } = null!;
    public FileGroup? FileGroup { get; set; } = null!;
    public Author User { get; set; } = null!;
}