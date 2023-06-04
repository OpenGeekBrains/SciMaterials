using Fluxor;

namespace SciMaterials.UI.BWASM.States.ContentTypes.Behavior;

public static class FilesContentTypesFilterReducers
{
    [ReducerMethod]
    public static FilesContentTypesFilterState LoadContentTypes(FilesContentTypesFilterState state, FilesContentTypesFilterActions.LoadContentTypesResultAction action)
    {
        var updateTime = action.ContentTypes.IsDefaultOrEmpty ? state.LastUpdated : DateTime.UtcNow;
        // TODO: remove not more existed content types from selected list
        return state with { ContentTypes = action.ContentTypes, LastUpdated = updateTime, IsLoading = false };
    }

    [ReducerMethod]
    public static FilesContentTypesFilterState UpdateContentTypesFilter(FilesContentTypesFilterState state, FilesContentTypesFilterActions.UpdateContentTypesFilterAction action)
    {
        return state with
        {
            Selected = action.Selected,
            Filter = string.Join(", ", action.Selected.Select(x => x.Extension))
        };
    }

    [ReducerMethod]
    public static FilesContentTypesFilterState LoadContentTypes(FilesContentTypesFilterState state, FilesContentTypesFilterActions.LoadContentTypesStartAction action)
    {
        // TODO: remove not more existed content types from selected list
        return state with { IsLoading = true };
    }
}