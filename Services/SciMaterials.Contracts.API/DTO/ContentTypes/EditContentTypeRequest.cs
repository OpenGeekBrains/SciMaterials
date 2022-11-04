namespace SciMaterials.Contracts.API.DTO.ContentTypes;

public class EditContentTypeRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
}