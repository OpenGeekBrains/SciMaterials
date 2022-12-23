using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoriesStorage
    {
        [Inject] private ICategoriesClient _CategoriesClient { get; set; }
        [Inject] private NavigationManager _NavigationManager { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }
        private IEnumerable<GetCategoryResponse>? _Categories = new List<GetCategoryResponse>();
        private string _Icon_CSharp = "icons/c_sharp.png";
        private bool _IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await _CategoriesClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                var data = result.Data;
                _Categories = data;
                _IsLoading  = false;
            }
        }
        
        private void OnCategoryClick(Guid? CategoryId)
        {
            _NavigationManager.NavigateTo($"/categories_storage/{CategoryId}");
        }
        
        private async Task OnAddClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey     = true,
                MaxWidth             = MaxWidth.Medium, 
                FullWidth            = true, 
                DisableBackdropClick = true
            };

            await _DialogService.ShowAsync<CategoriesAddDialog>(
                title: "Add new category",
                options: options).ConfigureAwait(false);
        }
        
        private async Task OnSearchAsync(string text)
        {
            var result = await _CategoriesClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                var data = result.Data;
                data = data.Where(element =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return true;
                    if (element.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (element.Description.Contains(text, StringComparison.OrdinalIgnoreCase))
                        return true;

                    return false;
                }).ToArray();
                
                _Categories = data;
            }
        }

        private async Task OnRefreshClickAsync()
        {
            await OnInitializedAsync();
        }
    }
}