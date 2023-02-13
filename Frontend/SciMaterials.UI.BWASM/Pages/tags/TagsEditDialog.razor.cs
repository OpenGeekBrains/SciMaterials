using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;
using SciMaterials.UI.BWASM.Pages.urls;

namespace SciMaterials.UI.BWASM.Pages.tags;

public partial class TagsEditDialog
{
    [Parameter] public DialogTypes DialogType { get; set; }
    [Parameter] public EditTagRequest EditTagsModel { get; set; }
    [Inject] private ITagsClient _TagsClient { get; set; }
    [Inject] private ISnackbar _Snackbar { get; set; }
    [Inject] private IDialogService _DialogService { get; set; }
    [CascadingParameter] private MudDialogInstance _MudDialog { get; set; }

    private async Task OnEditSaveClickAsync()
    {
        var result = await _TagsClient.EditAsync(EditTagsModel).ConfigureAwait(false);
        if (result.Succeeded)
        {
            _Snackbar.Add("Tag has been saved", Severity.Success);
            _MudDialog.Close();
        }
        else
        {
            _Snackbar.Add("Tag has NOT been saved!", Severity.Error);
        }
    }

    private void OnCancelClick()
    {
        _MudDialog.Cancel();
    }
}
