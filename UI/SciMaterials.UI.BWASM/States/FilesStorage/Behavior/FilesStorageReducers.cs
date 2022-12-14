using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesStorage.Behavior;

public static class FilesStorageReducers
{
    [ReducerMethod]
    public static FilesStorageState LoadFiles(FilesStorageState state, FilesStorageActions.LoadFilesResultAction action)
    {
        var updateTime = action.Files.IsDefaultOrEmpty ? state.LastUpdated : DateTime.UtcNow;
        return state with { Files = action.Files, LastUpdated = updateTime, IsLoading = false };
    }

    [ReducerMethod]
    public static FilesStorageState DeleteFile(FilesStorageState state, FilesStorageActions.DeleteFileResultAction action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }

    [ReducerMethod]
    public static FilesStorageState LoadFilesStart(FilesStorageState state, FilesStorageActions.LoadFilesStartAction action)
    {
        return state with { IsLoading = true };
    }
}