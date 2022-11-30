using Fluxor;

namespace SciMaterials.UI.BWASM.States.Authors.Behavior;

public static class AuthorsReducers
{
    [ReducerMethod]
    public static AuthorsState LoadAuthors(AuthorsState state, AuthorsActions.LoadAuthorsResultAction action)
    {
        return state with { Authors = action.Authors };
    }
}