using Fluxor;

namespace SciMaterials.UI.BWASM.States.FilesStorage.Behavior;

public static class FilesStorageReducers
{
    [ReducerMethod]
    public static FilesStorageState LoadFiles(FilesStorageState state, FilesStorageActions.LoadFilesResultAction action)
    {
        return state with { Files = action.Files };
    }

    [ReducerMethod]
    public static FilesStorageState DeleteFile(FilesStorageState state, FilesStorageActions.DeleteFileResultAction action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }
}