using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.States.Categories.Behavior;

public class FilesCategoriesEffects
{
    private readonly ICategoriesClient _categoriesClient;

    public FilesCategoriesEffects(ICategoriesClient categoriesClient)
    {
        _categoriesClient = categoriesClient;
    }

    [EffectMethod(typeof(FilesCategoriesActions.LoadCategoriesAction))]
    public async Task LoadCategories(IDispatcher dispatcher)
    {
        var result = await _categoriesClient.GetAllAsync();
        if (!result.Succeeded)
            // TODO: handle failure response
            return;

        var categories = result.Data?.Select(x => new FileCategory(x.Id, x.Name, x.Description, x.ParentId)).ToImmutableArray() ?? ImmutableArray<FileCategory>.Empty;
        dispatcher.Dispatch(FilesCategoriesActions.LoadCategoriesResult(categories));
        await BuildTree(dispatcher, categories);
    }

    private Task BuildTree(IDispatcher dispatcher, ImmutableArray<FileCategory> categories)
    {
        if (categories.IsEmpty) return Task.CompletedTask;

        var rootCategories = categories.Where(x => x.ParentId is null).ToList();

        HashSet<TreeFileCategory> tree = new();
        foreach (FileCategory category in rootCategories)
        {
            tree.Add(new(category, BuildBranch(categories, category.Id)));
        }

        dispatcher.Dispatch(FilesCategoriesActions.BuildTreeResult(tree));
        return Task.CompletedTask;
    }

    private HashSet<TreeFileCategory> BuildBranch(IReadOnlyCollection<FileCategory> categories, Guid root)
    {
        var inner = categories.Where(x => x.ParentId == root);
        HashSet<TreeFileCategory> branch = new();
        foreach (var category in inner)
            branch.Add(new(category, BuildBranch(categories, category.Id)));

        return branch;
    }
}