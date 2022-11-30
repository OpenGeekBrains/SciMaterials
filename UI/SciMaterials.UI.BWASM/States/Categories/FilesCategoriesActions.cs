using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.States.Categories;

public static class FilesCategoriesActions
{
    public record struct BuildTreeResultAction(HashSet<TreeFileCategory> Tree);
    public record struct LoadCategoriesAction;
    public record struct LoadCategoriesResultAction(ImmutableArray<FileCategory> Categories);

    public static BuildTreeResultAction BuildTreeResult(HashSet<TreeFileCategory> tree) => new(tree);
    public static LoadCategoriesAction LoadCategories() => new();
    public static LoadCategoriesResultAction LoadCategoriesResult(ImmutableArray<FileCategory> categories) => new(categories);
}