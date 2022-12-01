using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoriesStorage
    {
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetCategoryResponse>? _Elements = new List<GetCategoryResponse>();
        private string _Icon_CSharp = "icons/c_sharp.png";

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await CategoriesClient.GetAllAsync();
            if (result.Succeeded)
            {
                var data = result.Data;
                _Elements = data;
            }
            else
            {
                _Elements = new GetCategoryResponse[]{};
            }
        }
        
        private void OnClickCategory(Guid? id)
        {
            NavigationManager.NavigateTo($"/categories_storage/{id}");
        }
        
        private async Task OnSearchAsync(string text)
        {
            var result = await CategoriesClient.GetAllAsync().ConfigureAwait(false);
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
                
                _Elements = data;
            }
        }

        private async Task OnClickRefresh()
        {
            await OnInitializedAsync();
        }
    }
}