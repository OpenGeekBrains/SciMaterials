namespace SciMaterials.Contracts.API.DTO.ContentTypes;

public class AddContentTypeRequest
{
    public string Name { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
}