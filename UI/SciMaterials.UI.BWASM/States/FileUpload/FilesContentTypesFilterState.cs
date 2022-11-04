using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.ContentTypes;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record FilesContentTypesFilterState(ImmutableArray<ContentTypeState> ContentTypes)
{
    public FilesContentTypesFilterState() : this(ImmutableArray<ContentTypeState>.Empty) { }

    public ImmutableArray<ContentTypeState> Selected { get; init; } = ImmutableArray<ContentTypeState>.Empty;
    public string Filter { get; init; } = string.Empty;
}

public record ContentTypeState(Guid Id, string Extension, string Name);

public record struct LoadContentTypes;
public record struct LoadContentTypesResult(ImmutableArray<ContentTypeState> ContentTypes);
public record struct UpdateContentTypesFilter(ImmutableArray<ContentTypeState> Selected);

public class FilesContentTypesFilterEffects
{
    private readonly IContentTypesClient _contentTypesClient;

    public FilesContentTypesFilterEffects(IContentTypesClient contentTypesClient)
    {
        _contentTypesClient = contentTypesClient;
    }

    [EffectMethod(typeof(LoadContentTypes))]
    public async Task LoadContentTypes(IDispatcher dispatcher)
    {
        var result = await _contentTypesClient.GetAllAsync();
        if (!result.Succeeded)
        {
            // TODO: handle failure
            return;
        }

        var data = result.Data?.Select(x => new ContentTypeState(x.Id, x.FileExtension, x.Name)).ToImmutableArray() ?? ImmutableArray<ContentTypeState>.Empty;
        dispatcher.Dispatch(new LoadContentTypesResult(data));
    }
}


public static class FilesContentTypesFilterReducers
{
    [ReducerMethod]
    public static FilesContentTypesFilterState LoadContentTypes(FilesContentTypesFilterState state, LoadContentTypesResult action)
    {
        // TODO: remove not more existed content types from selected list
        return state with { ContentTypes = action.ContentTypes };
    }

    [ReducerMethod]
    public static FilesContentTypesFilterState UpdateContentTypesFilter(FilesContentTypesFilterState state, UpdateContentTypesFilter action)
    {
        return state with
        {
            Selected = action.Selected,
            Filter = string.Join(", ", action.Selected.Select(x=>x.Extension))
        };
    }
}