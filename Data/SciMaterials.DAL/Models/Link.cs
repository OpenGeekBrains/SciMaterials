using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models
{
    public class Link : BaseModel
    {
        public string SourceAddress { get; set; } = null!;
        public string Hash { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public string? Description { get; set; }

        public Author Author { get; set; } = null!;
    }
}