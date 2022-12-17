using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesStorage;

[FeatureState]
public record FilesStorageState(ImmutableArray<FileStorageState> Files) : CachedState
{
    public FilesStorageState() : this(ImmutableArray<FileStorageState>.Empty) { }

    public override bool IsNotTimeToUpdateData() => !Files.IsDefaultOrEmpty && base.IsNotTimeToUpdateData();
}
