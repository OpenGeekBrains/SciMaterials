namespace SciMaterials.Contracts.API.DTO.Files;

public class UploadFileRequest
{
    public string Name { get; set; } = string.Empty;
    public string ShortInfo { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Size { get; set; }
    public string ContentTypeName { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string Categories { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}