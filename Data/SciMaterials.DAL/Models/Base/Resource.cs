using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.Domain.Enums;

namespace SciMaterials.DAL.Models.Base;

[Table("Resources")]
public abstract class Resource : NamedModel
{
    public string? ShortInfo { get; set; }
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Author Author { get; set; } = null!;
    public abstract ResourceType ResourceType { get; }

    public ICollection<Comment>? Comments { get; set; } = new HashSet<Comment>();
    public ICollection<Tag>? Tags { get; set; } = new HashSet<Tag>();
    public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}