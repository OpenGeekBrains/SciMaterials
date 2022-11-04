namespace SciMaterials.Contracts.API.DTO.Files;

public class EditFileRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ShortInfo { get; set; } = null!;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string Categories { get; set; } = null!;
}