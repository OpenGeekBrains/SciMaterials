using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoryView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        private IEnumerable<GetCategoryResponse> Elements = new List<GetCategoryResponse>();
        
        protected override async Task OnInitializedAsync()
        {
            var result = await CategoriesClient.GetByIdAsync(Id);
            
            Elements = new List<GetCategoryResponse>() { result.Data };
        }
    }
}