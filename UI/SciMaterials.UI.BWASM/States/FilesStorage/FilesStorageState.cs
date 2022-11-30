using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesStorage;

[FeatureState]
public record FilesStorageState(ImmutableArray<FileStorageState> Files)
{
    public FilesStorageState() : this(ImmutableArray<FileStorageState>.Empty) { }
}