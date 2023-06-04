using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.ContentTypes;

namespace SciMaterials.UI.BWASM.States.ContentTypes.Behavior;

public class FilesContentTypesFilterEffects
{
    private readonly IContentTypesClient _contentTypesClient;
    private readonly IState<FilesContentTypesFilterState> _ContentTypesState;

    public FilesContentTypesFilterEffects(IContentTypesClient ContentTypesClient, IState<FilesContentTypesFilterState> ContentTypesState)
    {
        _contentTypesClient     = ContentTypesClient;
        _ContentTypesState = ContentTypesState;
    }

    [EffectMethod(typeof(FilesContentTypesFilterActions.LoadContentTypesAction))]
    public async Task LoadContentTypes(IDispatcher dispatcher)
    {
        if(_ContentTypesState.Value.IsNotTimeToUpdateData()) return;

        await ForceReloadContentTypes(dispatcher);
    }

    [EffectMethod(typeof(FilesContentTypesFilterActions.ForceReloadContentTypesAction))]
    public async Task ForceReloadContentTypes(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(FilesContentTypesFilterActions.LoadContentTypesStart());
        var result = await _contentTypesClient.GetAllAsync();
        if (!result.Succeeded)
            // TODO: handle failure
            return;

        var data = result.Data?.Select(x => new ContentTypeState(x.Id, x.FileExtension, x.Name)).ToImmutableArray() ?? ImmutableArray<ContentTypeState>.Empty;
        dispatcher.Dispatch(FilesContentTypesFilterActions.LoadContentTypesResult(data));
    }
}