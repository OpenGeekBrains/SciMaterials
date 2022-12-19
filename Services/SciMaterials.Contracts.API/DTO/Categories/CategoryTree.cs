namespace SciMaterials.Contracts.API.DTO.Categories;

public record CategoryTree(Guid? Id, string Name, IEnumerable<CategoryTreeInfo> SubCategories)
{
    public IEnumerable<CategoryTreeInfo> SubCategories { get; init; } = SubCategories ?? Enumerable.Empty<CategoryTreeInfo>();
}
