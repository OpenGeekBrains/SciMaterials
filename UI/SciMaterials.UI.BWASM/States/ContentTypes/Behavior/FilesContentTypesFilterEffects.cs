using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.ContentTypes;

namespace SciMaterials.UI.BWASM.States.ContentTypes.Behavior;

public class FilesContentTypesFilterEffects
{
    private readonly IContentTypesClient _contentTypesClient;

    public FilesContentTypesFilterEffects(IContentTypesClient contentTypesClient)
    {
        _contentTypesClient = contentTypesClient;
    }

    [EffectMethod(typeof(FilesContentTypesFilterActions.LoadContentTypesAction))]
    public async Task LoadContentTypes(IDispatcher dispatcher)
    {
        var result = await _contentTypesClient.GetAllAsync();
        if (!result.Succeeded)
            // TODO: handle failure
            return;

        var data = result.Data?.Select(x => new ContentTypeState(x.Id, x.FileExtension, x.Name)).ToImmutableArray() ?? ImmutableArray<ContentTypeState>.Empty;
        dispatcher.Dispatch(FilesContentTypesFilterActions.LoadContentTypesResult(data));
    }
}