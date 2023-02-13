using System.Collections.Immutable;

using SciMaterials.UI.BWASM.States.UploadFilesForm;

namespace SciMaterials.UI.BWASM.States.FilesUploadHistory;

public static class FilesUploadHistoryActions
{
    public record struct RegisterMultipleFilesUploadAction(ImmutableArray<FileUploadState> Files);

    public record struct FileUploadingAction(Guid Id);
    public record struct FileUploadedAction(Guid Id);
    public record struct FileUploadFailedAction(Guid Id, string ErrorCode);
    public record struct FileUploadCanceledAction(Guid Id);
    public record struct DeleteFileUploadAction(Guid Id);
    public record struct ChangeCategoryOfFileUploadAction(Guid Id, string CategoryName, Guid CategoryId);
    public record struct UpdateFileStateFromEditFormAction(Guid Id, UploadFilesFormState Form);

    public static RegisterMultipleFilesUploadAction RegisterFilesUpload(ImmutableArray<FileUploadState> files) => new(files);
    public static FileUploadingAction FileUploading(Guid id) => new(id);
    public static FileUploadedAction FileUploaded(Guid id) => new(id);
    public static FileUploadFailedAction FileUploadFailed(Guid id, string errorCode) => new(id, errorCode);
    public static FileUploadCanceledAction FileUploadCanceled(Guid id) => new(id);
    public static DeleteFileUploadAction DeleteFileUpload(Guid id) => new(id);
    public static ChangeCategoryOfFileUploadAction ChangeCategory(Guid id, string categoryName, Guid categoryId) => new(id, categoryName, categoryId);
    public static UpdateFileStateFromEditFormAction UpdateFileStateFromForm(Guid id, UploadFilesFormState form) => new(id, form);
}