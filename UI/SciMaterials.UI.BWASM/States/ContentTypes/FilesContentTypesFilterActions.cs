﻿using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.States.ContentTypes;

public static class FilesContentTypesFilterActions
{
    public record struct LoadContentTypesAction;
    public record struct ForceReloadContentTypesAction;
    public record struct LoadContentTypesStartAction;
    public record struct LoadContentTypesResultAction(ImmutableArray<ContentTypeState> ContentTypes);
    public record struct UpdateContentTypesFilterAction(ImmutableArray<ContentTypeState> Selected);

    public static LoadContentTypesAction LoadContentTypes() => new();
    public static ForceReloadContentTypesAction ForceReloadContentTypes() => new();
    public static LoadContentTypesStartAction LoadContentTypesStart() => new();
    public static LoadContentTypesResultAction LoadContentTypesResult(ImmutableArray<ContentTypeState> contentTypes) => new(contentTypes);
    public static UpdateContentTypesFilterAction UpdateFilter(ImmutableArray<ContentTypeState> selected) => new(selected);
}