using System.Collections.Immutable;

using Microsoft.AspNetCore.Components.Forms;

using SciMaterials.UI.BWASM.States.Authors;
using SciMaterials.UI.BWASM.States.Categories;

namespace SciMaterials.UI.BWASM.States.UploadFilesForm;

public static class UploadFilesFormActions
{
    public record struct UpdateShortInfoAction(string ShortInfo);
    public record struct UpdateDescriptionAction(string Description);
    public record struct ChangeCategoryAction(CategoryInfo Category);
    public record struct ChangeAuthorAction(AuthorInfo Author);

    public record struct ClearFormAction;

    public record struct AddFilesAction(IEnumerable<IBrowserFile> Files);
    public record struct RemoveFileAction(Guid Id);

    public record struct RegisterUploadDataAction(
        string ShortInfo,
        string? Description,
        CategoryInfo Category,
        AuthorInfo Author,
        ImmutableArray<FileData> Files);

    public static UpdateShortInfoAction UpdateShortInfo(string shortInfo) => new(shortInfo);
    public static UpdateDescriptionAction UpdateDescription(string description) => new(description);
    public static ChangeCategoryAction ChangeCategory(TreeFileCategory category) => new(new(category.Id, category.Name));
    public static ChangeAuthorAction ChangeAuthor(AuthorState author) => new(new(author.Id, author.Name, string.Empty));
    public static ClearFormAction ClearForm() => new();
    public static AddFilesAction AddFiles(IEnumerable<IBrowserFile> files) => new(files);
    public static RemoveFileAction RemoveFile(Guid id) => new(id);
    public static RegisterUploadDataAction RegisterUploadData(UploadFilesFormState form) => new(
        ShortInfo: form.ShortInfo,
        Description: form.Description,
        Category: form.Category,
        Author: form.Author,
        Files: form.Files);
}