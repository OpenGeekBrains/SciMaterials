namespace SciMaterials.Contracts.API.DTO.Urls;

public class AddUrlRequest
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Link { get; set; }
    public string Tags { get; set; } = null!;
    public string Categories { get; set; } = null!;
}