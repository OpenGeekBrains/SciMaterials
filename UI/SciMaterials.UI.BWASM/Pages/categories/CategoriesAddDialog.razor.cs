using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.UI.BWASM.Pages.urls;

namespace SciMaterials.UI.BWASM.Pages.categories;

public partial class CategoriesAddDialog
{
    [Parameter] public DialogTypes DialogType { get; set; }
    [Parameter] public AddCategoryRequest AddCategoryModel { get; set; } = new();
    [Inject] private ICategoriesClient _CategoriesClient { get; set; }
    [Inject] private ISnackbar _Snackbar { get; set; }
    [CascadingParameter] private MudDialogInstance _MudDialog { get; set; }
    
    private async Task OnSaveClickAsync()
    {
        var result = await _CategoriesClient.AddAsync(AddCategoryModel).ConfigureAwait(false);
        if (result.Succeeded)
        {
            _Snackbar.Add("Category has been saved", Severity.Success);
            _MudDialog.Close();
        }
        else
        {
            _Snackbar.Add("Category has NOT been saved!", Severity.Error);
        }
    }

    private void OnCancelClick()
    {
        _MudDialog.Cancel();
    }
}
