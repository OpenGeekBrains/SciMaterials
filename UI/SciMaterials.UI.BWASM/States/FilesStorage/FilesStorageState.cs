using System.Collections.Immutable;

using Fluxor;

using SciMaterials.Contracts.WebApi.Clients.Files;

namespace SciMaterials.UI.BWASM.States.FilesStorage;

[FeatureState]
public record FilesStorageState(ImmutableArray<FileStorageState> Files)
{
    public FilesStorageState() : this(ImmutableArray<FileStorageState>.Empty) { }
}

public record FileStorageState(Guid Id, string FileName, string Category, long Size, string Url);

public record struct LoadFiles();
public record struct LoadFilesResult(ImmutableArray<FileStorageState> Files);
public record struct DeleteFile(Guid Id);
public record struct DeleteFileResult(Guid Id);

public class FilesStorageEffects
{
    private readonly IFilesClient _filesClient;

    public FilesStorageEffects(IFilesClient filesClient)
    {
        _filesClient = filesClient;
    }

    [EffectMethod(typeof(LoadFiles))]
    public async Task LoadFiles(IDispatcher dispatcher)
    {
        var result = await _filesClient.GetAllAsync();
        if (!result.Succeeded)
        {
            dispatcher.Dispatch(new LoadFilesResult());
            return;
        }

        var files = result.Data!.Select(x => new FileStorageState(x.Id, x.Name, x.Categories, x.Size, x.Url)).ToImmutableArray();
        dispatcher.Dispatch(new LoadFilesResult(files));
    }

    [EffectMethod]
    public async Task DeleteFile(DeleteFile action, IDispatcher dispatcher)
    {
        var result = await _filesClient.DeleteAsync(action.Id, CancellationToken.None);
        if (!result.Succeeded) return;

        dispatcher.Dispatch(new DeleteFileResult(action.Id));
    }
}

public static class FilesStorageReducers
{
    [ReducerMethod]
    public static FilesStorageState LoadFiles(FilesStorageState state, LoadFilesResult action)
    {
        return state with { Files = action.Files };
    }

    [ReducerMethod]
    public static FilesStorageState DeleteFile(FilesStorageState state, DeleteFileResult action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }
}