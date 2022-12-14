using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Files;

namespace SciMaterials.UI.BWASM.States.FilesStorage.Behavior;

public class FilesStorageEffects
{
    private readonly IFilesClient _filesClient;
    private readonly IState<FilesStorageState> _FilesStorageState;

    public FilesStorageEffects(IFilesClient filesClient, IState<FilesStorageState> FilesStorageState)
    {
        _filesClient            = filesClient;
        _FilesStorageState = FilesStorageState;
    }

    [EffectMethod(typeof(FilesStorageActions.LoadFilesAction))]
    public async Task LoadFiles(IDispatcher dispatcher)
    {
        if (_FilesStorageState.Value.IsNotTimeToUpdateData()) return;

        dispatcher.Dispatch(FilesStorageActions.LoadFilesStart());
        var result = await _filesClient.GetAllAsync();
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
        var result = await _filesClient.DeleteAsync(action.Id, CancellationToken.None);
        if (!result.Succeeded) return;

        dispatcher.Dispatch(FilesStorageActions.DeleteFileResult(action.Id));
    }
}