using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.UI.BWASM.States.Authors;

[FeatureState]
public record AuthorsState(ImmutableArray<AuthorState> Authors)
{
    public AuthorsState() : this(ImmutableArray<AuthorState>.Empty) { }
}

public record AuthorState(Guid Id, string Name);

public record struct LoadAuthors;
public record struct LoadAuthorsResult(ImmutableArray<AuthorState> Authors);

public class AuthorsEffects
{
    private readonly IAuthorsClient _authorsClient;

    public AuthorsEffects(IAuthorsClient authorsClient)
    {
        _authorsClient = authorsClient;
    }

    [EffectMethod(typeof(LoadAuthors))]
    public async Task LoadAuthors(IDispatcher dispatcher)
    {
        var result = await _authorsClient.GetAllAsync();
        if (!result.Succeeded)
        {
            // TODO: handle failure
            return;
        }

        var data = result.Data?.Select(x => new AuthorState(x.Id, x.Name)).ToImmutableArray() ?? ImmutableArray<AuthorState>.Empty;
        dispatcher.Dispatch(new LoadAuthorsResult(data));
    }
}

public static class AuthorsReducers
{
    [ReducerMethod]
    public static AuthorsState LoadAuthors(AuthorsState state, LoadAuthorsResult action)
    {
        return state with { Authors = action.Authors };
    }
}