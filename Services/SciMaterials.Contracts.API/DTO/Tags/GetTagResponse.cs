namespace SciMaterials.Contracts.API.DTO.Tags;

public class GetTagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}