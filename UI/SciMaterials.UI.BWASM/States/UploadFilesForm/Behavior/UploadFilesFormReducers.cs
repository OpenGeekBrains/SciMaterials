using Fluxor;

namespace SciMaterials.UI.BWASM.States.UploadFilesForm.Behavior;

public static class UploadFilesFormReducers
{
    [ReducerMethod]
    public static UploadFilesFormState UpdateShortInfo(UploadFilesFormState state, UploadFilesFormActions.UpdateShortInfoAction action)
    {
        return state with { ShortInfo = action.ShortInfo };
    }

    [ReducerMethod]
    public static UploadFilesFormState UpdateDescription(UploadFilesFormState state, UploadFilesFormActions.UpdateDescriptionAction action)
    {
        return state with { Description = action.Description };
    }

    [ReducerMethod]
    public static UploadFilesFormState ChangeCategory(UploadFilesFormState state, UploadFilesFormActions.ChangeCategoryAction action)
    {
        return state with { Category = action.Category };
    }

    [ReducerMethod]
    public static UploadFilesFormState ChangeAuthor(UploadFilesFormState state, UploadFilesFormActions.ChangeAuthorAction action)
    {
        return state with { Author = action.Author };
    }

    [ReducerMethod]
    public static UploadFilesFormState AddFiles(UploadFilesFormState state, UploadFilesFormActions.AddFilesAction action)
    {
        return state with { Files = state.Files.AddRange(action.Files.Select(x => new FileData(x))) };
    }

    [ReducerMethod(typeof(UploadFilesFormActions.ClearFormAction))]
    public static UploadFilesFormState ClearForm(UploadFilesFormState state)
    {
        return UploadFilesFormState.Empty;
    }

    [ReducerMethod]
    public static UploadFilesFormState RemoveFile(UploadFilesFormState state, UploadFilesFormActions.RemoveFileAction action)
    {
        if (state.Files.FirstOrDefault(x => x.Id == action.Id) is not { } toDelete) return state;

        var afterDelete = state.Files.Remove(toDelete);
        return state with { Files = afterDelete };
    }
}