using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.Authors;

[FeatureState]
public record AuthorsState(ImmutableArray<AuthorState> Authors)
{
    public AuthorsState() : this(ImmutableArray<AuthorState>.Empty) { }
}