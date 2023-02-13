namespace SciMaterials.UI.BWASM.States.Categories;

public record TreeFileCategory(Guid Id, string Name, string? Description, Guid? ParentId, HashSet<TreeFileCategory> Categories)
{
    public TreeFileCategory(FileCategory source, HashSet<TreeFileCategory> categories)
        : this(source.Id, source.Name, source.Description, source.ParentId, categories)
    {
    }
};