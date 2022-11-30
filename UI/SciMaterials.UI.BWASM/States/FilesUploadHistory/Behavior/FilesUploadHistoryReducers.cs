using System.Collections.Immutable;

using Fluxor;

using SciMaterials.UI.BWASM.Extensions;

namespace SciMaterials.UI.BWASM.States.FilesUploadHistory.Behavior;

public static class FilesUploadHistoryReducers
{
    [ReducerMethod]
    public static FilesUploadHistoryState RegisterFilesUpload(FilesUploadHistoryState state, FilesUploadHistoryActions.RegisterMultipleFilesUploadAction action)
    {
        return state with
        {
            Files = state.Files.AddRange(action.Files)
        };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploading(FilesUploadHistoryState state, FilesUploadHistoryActions.FileUploadingAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with { State = UploadState.Uploading },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploaded(FilesUploadHistoryState state, FilesUploadHistoryActions.FileUploadedAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with { State = UploadState.Uploaded },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadFailed(FilesUploadHistoryState state, FilesUploadHistoryActions.FileUploadFailedAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with { State = UploadState.Failure },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadCanceled(FilesUploadHistoryState state, FilesUploadHistoryActions.FileUploadCanceledAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with { State = UploadState.Canceled },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState DeleteFileUpload(FilesUploadHistoryState state, FilesUploadHistoryActions.DeleteFileUploadAction action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;
        toDelete.CancellationSource.Cancel();
        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState ChangeCategoryOfFileUpload(FilesUploadHistoryState state, FilesUploadHistoryActions.ChangeCategoryOfFileUploadAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with
            {
                CategoryName = action.CategoryName,
                CategoryId = action.CategoryId
            },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState UpdateFileStateFromEditForm(FilesUploadHistoryState state, FilesUploadHistoryActions.UpdateFileStateFromEditFormAction action)
    {
        return !state.Files.ReplaceOne(
            selector: x => x.Id == action.Id,
            replacement: x => x with
            {
                //FileName = action.Form.FileName,
                //Title = action.Form.ShortInfo,
                //CategoryName = action.Form.CategoryName,
                //CategoryId = action.Form.CategoryId,
                //AuthorName = action.Form.AuthorName,
                //AuthorId = action.Form.AuthorId
            },
            result: out ImmutableArray<FileUploadState> files)
            ? state
            : state with { Files = files };
    }
}