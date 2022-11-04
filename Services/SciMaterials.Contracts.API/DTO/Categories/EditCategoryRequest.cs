namespace SciMaterials.Contracts.API.DTO.Categories;

public class EditCategoryRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
}