using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Rating : BaseModel
{
    public Guid? ResourceId { get; set; }
    public Guid AuthorId { get; set; }
    public int RatingValue { get; set; }
    
    public Resource? Resource { get; set; } = null!;
    public Author User { get; set; } = null!;
}