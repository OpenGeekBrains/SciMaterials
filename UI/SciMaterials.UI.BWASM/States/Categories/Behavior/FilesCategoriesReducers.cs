using Fluxor;

namespace SciMaterials.UI.BWASM.States.Categories.Behavior;

public static class FilesCategoriesReducers
{
    [ReducerMethod]
    public static FilesCategoriesState LoadCategories(FilesCategoriesState state, FilesCategoriesActions.LoadCategoriesResultAction action)
    {
        return state with
        {
            Categories = action.Categories,
            // TODO: configure server call timeouts
            NextServerCall = DateTime.UtcNow.AddSeconds(40)
        };
    }

    [ReducerMethod]
    public static FilesCategoriesState BuildTree(FilesCategoriesState state, FilesCategoriesActions.BuildTreeResultAction action)
    {
        return state with { Tree = action.Tree };
    }
}