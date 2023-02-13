using Fluxor;

namespace SciMaterials.UI.BWASM.States.Categories.Behavior;

public static class FilesCategoriesReducers
{
    [ReducerMethod]
    public static FilesCategoriesState LoadCategories(FilesCategoriesState state, FilesCategoriesActions.LoadCategoriesResultAction action)
    {
        var updateTime = action.Categories.IsDefaultOrEmpty ? state.LastUpdated : DateTime.UtcNow;
        return state with { Categories = action.Categories, LastUpdated = updateTime, IsLoading = false };
    }

    [ReducerMethod]
    public static FilesCategoriesState BuildTree(FilesCategoriesState state, FilesCategoriesActions.BuildTreeResultAction action)
    {
        return state with { Tree = action.Tree };
    }

    [ReducerMethod]
    public static FilesCategoriesState LoadCategoriesStart(FilesCategoriesState state, FilesCategoriesActions.LoadCategoriesStartAction action)
    {
        return state with { IsLoading = true };
    }
}