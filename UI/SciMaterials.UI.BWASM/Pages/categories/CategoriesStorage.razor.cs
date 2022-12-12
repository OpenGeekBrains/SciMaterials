using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoriesStorage
    {
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetCategoryResponse>? _Categories = new List<GetCategoryResponse>();
        private string _Icon_CSharp = "icons/c_sharp.png";
        private bool _IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await CategoriesClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                var data = result.Data;
                _Categories = data;
                _IsLoading  = false;
            }
        }
        
        private void OnCategoryClick(Guid? CategoryId)
        {
            NavigationManager.NavigateTo($"/categories_storage/{CategoryId}");
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
                
                _Categories = data;
            }
        }

        private async Task OnRefreshClickAsync()
        {
            await OnInitializedAsync();
        }
    }
}