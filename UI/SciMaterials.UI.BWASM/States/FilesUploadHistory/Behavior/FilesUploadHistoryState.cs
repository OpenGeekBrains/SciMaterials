using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesUploadHistory.Behavior;

[FeatureState]
public record FilesUploadHistoryState(ImmutableArray<FileUploadState> Files)
{
    public static readonly FilesUploadHistoryState Empty = new();

    private FilesUploadHistoryState() : this(ImmutableArray<FileUploadState>.Empty) { }
}