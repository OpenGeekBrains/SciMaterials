using System.Collections.Immutable;

using Fluxor;

using Microsoft.AspNetCore.Components.Forms;

using SciMaterials.UI.BWASM.Extensions;

namespace SciMaterials.UI.BWASM.States.FileUpload;

[FeatureState]
public record FilesUploadHistoryState(ImmutableArray<FileUploadState> Files)
{
    public static readonly FilesUploadHistoryState Empty = new();

    private FilesUploadHistoryState() : this(ImmutableArray<FileUploadState>.Empty) { }
}

public record FileUploadState(IBrowserFile BrowserFile, string CategoryName = "", Guid CategoryId = default)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = BrowserFile.Name;
    public long Size { get; init; } = BrowserFile.Size;
    public IBrowserFile BrowserFile { get; init; } = BrowserFile;
    public string CategoryName { get; init; } = CategoryName;
    public Guid CategoryId { get; init; } = CategoryId;
    public string AuthorName { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentType { get; init; } = BrowserFile.ContentType;
    public string? Description { get; init; }
    public UploadState State { get; init; } = UploadState.Scheduled;
    public CancellationTokenSource? CancellationSource { get; init; }
}

public enum UploadState
{
    Scheduled,
    Uploading,
    Uploaded,
    Failure,
    Canceled
}

public record struct RegisterMultipleFilesUpload(ImmutableArray<FileUploadState> Files);

public record struct FileUploading(Guid Id);
public record struct FileUploaded(Guid Id);
public record struct FileUploadFailed(Guid Id, int ErrorCode);
public record struct FileUploadCanceled(Guid Id);
public record struct DeleteFileUpload(Guid Id);
public record struct ChangeCategoryOfFileUpload(Guid Id, string CategoryName, Guid CategoryId);
public record struct UpdateFileStateFromEditForm(Guid Id, UploadFilesFormState Form);

public static class FileUploadReducers
{
    [ReducerMethod]
    public static FilesUploadHistoryState RegisterFilesUpload(FilesUploadHistoryState state, RegisterMultipleFilesUpload action)
    {
        return state with
        {
            Files = state.Files.AddRange(action.Files)
        };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploading(FilesUploadHistoryState state, FileUploading action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploading},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploaded(FilesUploadHistoryState state, FileUploaded action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Uploaded},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadFailed(FilesUploadHistoryState state, FileUploadFailed action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Failure},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState FileUploadCanceled(FilesUploadHistoryState state, FileUploadCanceled action)
    {
        return !state.Files.ReplaceOne(
                selector: x => x.Id == action.Id,
                replacement: x => x with { State = UploadState.Canceled},
                result: out ImmutableArray<FileUploadState> files)
            ? state 
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState DeleteFileUpload(FilesUploadHistoryState state, DeleteFileUpload action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not {} toDelete) return state;
        toDelete.CancellationSource.Cancel();
        var afterDelete = state.Files.Remove(toDelete);
        return state with {Files = afterDelete };
    }

    [ReducerMethod]
    public static FilesUploadHistoryState ChangeCategoryOfFileUpload(FilesUploadHistoryState state, ChangeCategoryOfFileUpload action)
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
            : state with {Files = files};
    }

    [ReducerMethod]
    public static FilesUploadHistoryState UpdateFileStateFromEditForm(FilesUploadHistoryState state, UpdateFileStateFromEditForm action)
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
            : state with {Files = files};
    }
}