using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.UI.BWASM.States.Categories;

namespace SciMaterials.UI.BWASM.Pages.categories
{
    public partial class CategoryView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ICategoriesClient CategoriesClient { get; set; }
        [Inject] private IFilesClient FilesClient { get; set; }
        [Inject] private IDispatcher Dispatcher { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }
        [Inject] private IState<FilesCategoriesState> State { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        
        private IEnumerable<GetFileResponse>? _Files = new List<GetFileResponse>();
        private GetCategoryResponse? _CurrentCategory { get; set; }
        private string? _Icon_CSharp = "icons/c_sharp.png";
        private string? _SearchString { get; set; }
        private bool _IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
            
            var categoriesResult = await CategoriesClient.GetByIdAsync(Id).ConfigureAwait(false);
            if (categoriesResult.Succeeded)
            {
                _CurrentCategory = categoriesResult.Data;
                
                var filesResult = await FilesClient.GetAllAsync().ConfigureAwait(false);
                if (filesResult.Succeeded)
                {
                    _Files = filesResult.Data;

                    _IsLoading = false;
                }
            }
        }
        
        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
            
            var result = await CategoriesClient.GetByIdAsync(Id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _CurrentCategory = result.Data;
            }
        }
        
        private async Task OnCategoryClick(Guid? CategoryId)
        {
            NavigationManager.NavigateTo($"/categories_storage/{CategoryId}");
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

            _DialogService.Show<CategoriesAddDialog>(
                title: "Add new category",
                options: options);
        }

        private void OnBackClick()
        {
            NavigationManager.NavigateTo("/categories_storage");
        }

        private Func<GetFileResponse, bool> _QuickSearch => (x) => 
        {
            if (string.IsNullOrWhiteSpace(_SearchString))
                return true;
            
            if (x.Name.ToString().Contains(_SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (x.Description.ToString().Contains(_SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        };
    }
}