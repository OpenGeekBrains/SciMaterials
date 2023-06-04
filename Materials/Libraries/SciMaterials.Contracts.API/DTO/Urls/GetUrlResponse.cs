namespace SciMaterials.Contracts.API.DTO.Urls;

public class GetUrlResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Link { get; set; } = null!;
}