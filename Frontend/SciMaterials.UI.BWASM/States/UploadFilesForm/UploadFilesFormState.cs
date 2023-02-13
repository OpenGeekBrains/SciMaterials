using System.Collections.Immutable;

using Fluxor;

namespace SciMaterials.UI.BWASM.States.UploadFilesForm;

[FeatureState]
public record UploadFilesFormState
{
    public static readonly UploadFilesFormState Empty = new();

    public Guid Id { get; init; } = Guid.NewGuid();
    public string ShortInfo { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;

    public CategoryInfo Category { get; init; }
    public AuthorInfo Author { get; init; }

    public ImmutableArray<FileData> Files { get; init; } = ImmutableArray<FileData>.Empty;
}