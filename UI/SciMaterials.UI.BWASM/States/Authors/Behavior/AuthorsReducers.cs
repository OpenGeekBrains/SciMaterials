using Fluxor;

namespace SciMaterials.UI.BWASM.States.Authors.Behavior;

public static class AuthorsReducers
{
    [ReducerMethod]
    public static AuthorsState LoadAuthors(AuthorsState state, AuthorsActions.LoadAuthorsResultAction action)
    {
        var updateTime = action.Authors.IsDefaultOrEmpty ? state.LastUpdated : DateTime.UtcNow;
        return state with { Authors = action.Authors, LastUpdated = updateTime, IsLoading = false };
    }

    [ReducerMethod]
    public static AuthorsState LoadAuthorsStart(AuthorsState state, AuthorsActions.LoadAuthorsStartAction action)
    {
        return state with { IsLoading = true};
    }
}