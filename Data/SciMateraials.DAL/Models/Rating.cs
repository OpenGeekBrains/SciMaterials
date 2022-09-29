namespace SciMaterials.DAL.Models;

public partial class Rating
{
    public Guid FileId { get; set; }
    public Guid UserId { get; set; }
    public int RatingValue { get; set; }

    public File File { get; set; } = null!;
    public User User { get; set; } = null!;
}
