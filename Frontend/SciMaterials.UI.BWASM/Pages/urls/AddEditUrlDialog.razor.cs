using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Urls;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.Pages.urls;
public enum DialogTypes
{
    Add = 0,
    Edit = 1
}
public partial class AddEditUrlDialog
{
    [Inject] private IUrlsClient UrlsClient { get; set; }
    [Inject] private ICategoriesClient CategoriesClient { get; set; }

    [Parameter] public DialogTypes DialogType { get; set; } 
    [Parameter] public AddUrlRequest AddEditUrlModel { get; set; } = new();
    [Parameter] public EditUrlRequest EditEditUrlModel { get; set; } = new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private IEnumerable<GetCategoryResponse> _categories = new List<GetCategoryResponse>();

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private Task SaveAsync()
    {
        //var response = await ProductManager.SaveAsync(AddEditProductModel);
        //if (response.Succeeded)
        //{
        //    _snackBar.Add(response.Messages[0], Severity.Success);
        //    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        //    MudDialog.Close();
        //}
        //else
        //{
        //    foreach (var message in response.Messages)
        //    {
        //        _snackBar.Add(message, Severity.Error);
        //    }
        //}
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var data = await CategoriesClient.GetAllAsync();
        if (data.Succeeded && data.Data is { } categories)
        {
            _categories = categories;
        }
    }

    private async Task<IEnumerable<Guid>> SearchCategories(string value)
    {
        if (string.IsNullOrEmpty(value))
            return _categories.Select(c => c.Id);

        return _categories.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}
