namespace SciMaterials.Contracts.API.Models;

public class FileMetadata
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public long Size { get; set; }
    public string ContentTypeName { get; set; } = null!;
    public string? Tags { get; set; }
    public string Categories { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public Guid AuthorId { get; set; }
}