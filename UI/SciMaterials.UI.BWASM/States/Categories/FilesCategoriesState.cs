using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.Categories;

[FeatureState]
public record FilesCategoriesState(ImmutableArray<FileCategory> Categories) : CachedState
{
    public FilesCategoriesState() : this(ImmutableArray<FileCategory>.Empty) { }
    
    public HashSet<TreeFileCategory> Tree { get; init; }
}