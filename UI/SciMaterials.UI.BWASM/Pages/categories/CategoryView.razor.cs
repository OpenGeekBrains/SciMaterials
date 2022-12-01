using Fluxor;
using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.UI.BWASM.States.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoryView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        [Inject] private IDispatcher Dispatcher { get; set; }
        [Inject] private IState<FilesCategoriesState> State { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetCategoryResponse>? _Elements = new List<GetCategoryResponse>();
        private string _Icon_CSharp = "icons/c_sharp.png";
        
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
            var result = await CategoriesClient.GetByIdAsync(Id).ConfigureAwait(false);
            
            _Elements = new List<GetCategoryResponse>() { result.Data };
        }

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
            var result = await CategoriesClient.GetByIdAsync(Id).ConfigureAwait(false);
            
            _Elements = new List<GetCategoryResponse>() { result.Data };
        }
        
        private async Task OnCategoryClick(Guid? categoryId)
        {
            NavigationManager.NavigateTo($"/categories_storage/{categoryId}");
        }

        private void OnBackClick()
        {
            NavigationManager.NavigateTo("/categories_storage");
        }
    }
}