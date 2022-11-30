using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.Categories;

[FeatureState]
public record FilesCategoriesState(ImmutableArray<FileCategory> Categories)
{
    public FilesCategoriesState() : this(ImmutableArray<FileCategory>.Empty) { }

    public DateTime NextServerCall { get; set; }
    public HashSet<TreeFileCategory> Tree { get; init; }
}