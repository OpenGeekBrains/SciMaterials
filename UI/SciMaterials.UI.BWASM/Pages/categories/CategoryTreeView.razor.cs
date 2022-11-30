using Fluxor;
using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.UI.BWASM.States.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoryTreeView
    {
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IDispatcher Dispatcher { get; set; }
        [Inject] private IState<FilesCategoriesState> State { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
        }

        private async Task OnCategoryClick(Guid categoryId)
        {
            NavigationManager.NavigateTo($"/categories_storage/{categoryId}");
        }
    }
}