using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.UI.BWASM.Pages.urls;

namespace SciMaterials.UI.BWASM.Pages.categories;

public partial class CategoriesDeleteDialog
{
    [Parameter] public DialogTypes DialogType { get; set; }
    [Parameter] public EditCategoryRequest DeleteCategoriesModel { get; set; }
    [Inject] private ICategoriesClient _CategoriesClient { get; set; }
    [Inject] private ISnackbar _Snackbar { get; set; }
    [Inject] private IDialogService _DialogService { get; set; }
    [CascadingParameter] private MudDialogInstance _MudDialog { get; set; }

    private async Task OnDeleteSaveClickAsync()
    {
        var result = await _CategoriesClient.DeleteAsync(DeleteCategoriesModel.Id).ConfigureAwait(false);
        if (result.Succeeded)
        {
            _Snackbar.Add("Category has been deleted", Severity.Success);
            _MudDialog.Close();
        }
        else
        {
            _Snackbar.Add("Category has NOT been deleted!", Severity.Error);
        }
    }

    private void OnCancelClick()
    {
        _MudDialog.Cancel();
    }
}
