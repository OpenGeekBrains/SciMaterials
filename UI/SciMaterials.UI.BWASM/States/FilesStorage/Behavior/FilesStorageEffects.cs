using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Files;

namespace SciMaterials.UI.BWASM.States.FilesStorage.Behavior;

public class FilesStorageEffects
{
    private readonly IFilesClient _FilesClient;
    private readonly IState<FilesStorageState> _FilesStorageState;

    public FilesStorageEffects(IFilesClient FilesClient, IState<FilesStorageState> FilesStorageState)
    {
        _FilesClient = FilesClient;
        _FilesStorageState = FilesStorageState;
    }

    [EffectMethod(typeof(FilesStorageActions.LoadFilesAction))]
    public async Task LoadFiles(IDispatcher dispatcher)
    {
        if (_FilesStorageState.Value.IsNotTimeToUpdateData()) return;

        await ForceReloadFiles(dispatcher);
    }

    [EffectMethod(typeof(FilesStorageActions.ForceReloadFilesAction))]
    public async Task ForceReloadFiles(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(FilesStorageActions.LoadFilesStart());
        var result = await _FilesClient.GetAllAsync();
        if (!result.Succeeded)
        {
            dispatcher.Dispatch(FilesStorageActions.LoadFilesResult(ImmutableArray<FileStorageState>.Empty));
            return;
        }

        var files = result.Data!.Select(x => new FileStorageState(x.Id, x.Name, x.Categories, x.Size, x.Url)).ToImmutableArray();
        dispatcher.Dispatch(FilesStorageActions.LoadFilesResult(files));
    }

    [EffectMethod]
    public async Task DeleteFile(FilesStorageActions.DeleteFileAction action, IDispatcher dispatcher)
    {
        var result = await _FilesClient.DeleteAsync(action.Id, CancellationToken.None);
        if (!result.Succeeded) return;

        dispatcher.Dispatch(FilesStorageActions.DeleteFileResult(action.Id));
    }
}