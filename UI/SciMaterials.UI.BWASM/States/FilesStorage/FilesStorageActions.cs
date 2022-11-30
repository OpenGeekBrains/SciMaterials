using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.States.FilesStorage;

public static class FilesStorageActions
{
    public record struct LoadFilesAction;
    public record struct LoadFilesResultAction(ImmutableArray<FileStorageState> Files);
    public record struct DeleteFileAction(Guid Id);
    public record struct DeleteFileResultAction(Guid Id);

    public static LoadFilesAction LoadFiles() => new();
    public static LoadFilesResultAction LoadFilesResult(ImmutableArray<FileStorageState> files) => new(files);
    public static DeleteFileAction DeleteFile(Guid id) => new(id);
    public static DeleteFileResultAction DeleteFileResult(Guid id) => new(id);
}