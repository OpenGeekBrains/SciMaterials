using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.UI.BWASM.States.Authors.Behavior;

public class AuthorsEffects
{
    private readonly IAuthorsClient _authorsClient;

    public AuthorsEffects(IAuthorsClient authorsClient)
    {
        _authorsClient = authorsClient;
    }

    [EffectMethod(typeof(AuthorsActions.LoadAuthorsAction))]
    public async Task LoadAuthors(IDispatcher dispatcher)
    {
        var result = await _authorsClient.GetAllAsync();
        if (!result.Succeeded)
            // TODO: handle failure
            return;

        var data = result.Data?.Select(x => new AuthorState(x.Id, x.Name)).ToImmutableArray() ?? ImmutableArray<AuthorState>.Empty;
        dispatcher.Dispatch(AuthorsActions.LoadAuthorsResult(data));
    }
}