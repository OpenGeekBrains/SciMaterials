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
        [Parameter] public Guid CategoryId { get; set; }
        [Inject] private ICategoriesClient _CategoriesClient { get; set; }
        [Inject] private IFilesClient _FilesClient { get; set; }
        [Inject] private IDispatcher _Dispatcher { get; set; }
        [Inject] private IState<FilesCategoriesState> _State { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }
        [Inject] private NavigationManager _NavigationManager { get; set; }

        private IEnumerable<GetFileResponse>? _Files = new List<GetFileResponse>();
        private GetCategoryResponse? _CurrentCategory { get; set; }
        private string? _Icon_CSharp = "icons/c_sharp.png";
        private string? _SearchString { get; set; }
        private bool _IsLoading = true;
        
        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            _Dispatcher.Dispatch(FilesCategoriesActions.LoadCategories());
            
            var categoriesResult = await _CategoriesClient.GetByIdAsync(CategoryId).ConfigureAwait(false);
            if (categoriesResult.Succeeded)
            {
                _CurrentCategory = categoriesResult.Data;
                
                var filesResult = await _FilesClient.GetAllAsync().ConfigureAwait(false);
                if (filesResult.Succeeded)
                {
                    _Files = filesResult.Data;

                    _IsLoading = false;
                }
            }
        }

        private async Task OnLinkClickAsync(Guid? FileId)
        {
            _NavigationManager.NavigateTo($"/filedetails/{FileId}");
        }

        private async Task OnCategoryClickAsync(Guid? CategoryId)
        {
            _NavigationManager.NavigateTo($"/categories_storage/{CategoryId}");
        }

        private void OnBackClick()
        {
            _NavigationManager.NavigateTo("/categories_storage");
        }
        
        private async Task OnEditClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey     = true,
                MaxWidth             = MaxWidth.Medium, 
                FullWidth            = true, 
                DisableBackdropClick = true
            };

            var parameters = new DialogParameters();
            parameters.Add(nameof(CategoriesEditDialog.EditCategoriesModel), new EditCategoryRequest() {Id = CategoryId});
            
            await _DialogService.ShowAsync<CategoriesEditDialog>(
                title: "Edit category name and description",
                options: options,
                parameters: parameters).ConfigureAwait(false);
        }
        
        private async Task OnDeleteClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey     = true,
                MaxWidth             = MaxWidth.Medium, 
                FullWidth            = true, 
                DisableBackdropClick = true
            };

            var parameters = new DialogParameters();
            parameters.Add(nameof(CategoriesDeleteDialog.DeleteCategoriesModel), new EditCategoryRequest() {Id = CategoryId});
            
            await _DialogService.ShowAsync<CategoriesDeleteDialog>(
                title: "Delete category",
                options: options,
                parameters: parameters).ConfigureAwait(false);
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