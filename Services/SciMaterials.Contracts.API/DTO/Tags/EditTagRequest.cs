namespace SciMaterials.Contracts.API.DTO.Tags;

public class EditTagRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}