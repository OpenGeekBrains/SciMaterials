using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.UI.BWASM.States.Authors.Behavior;

public class AuthorsEffects
{
    private readonly IAuthorsClient _AuthorsClient;
    private readonly IState<AuthorsState> _AuthorsState;

    public AuthorsEffects(IAuthorsClient AuthorsClient, IState<AuthorsState> AuthorsState)
    {
        _AuthorsClient     = AuthorsClient;
        _AuthorsState = AuthorsState;
    }

    [EffectMethod(typeof(AuthorsActions.LoadAuthorsAction))]
    public async Task LoadAuthors(IDispatcher dispatcher)
    {
        if(_AuthorsState.Value.IsNotTimeToUpdateData()) return;

        await ForceReloadAuthors(dispatcher);
    }

    [EffectMethod(typeof(AuthorsActions.ForceReloadAuthorsAction))]
    public async Task ForceReloadAuthors(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(AuthorsActions.LoadAuthorsStart());
        var result = await _AuthorsClient.GetAllAsync();
        if (!result.Succeeded)
        {
            // TODO: handle failure
            return;
        }

        var data = result.Data?.Select(x => new AuthorState(x.Id, x.Name)).ToImmutableArray() ?? ImmutableArray<AuthorState>.Empty;
        dispatcher.Dispatch(AuthorsActions.LoadAuthorsResult(data));
    }
}