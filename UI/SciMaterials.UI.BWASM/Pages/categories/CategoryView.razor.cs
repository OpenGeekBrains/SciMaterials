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
        [Inject] private IState<FilesCategoriesState> State { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetCategoryResponse>? _Elements = new List<GetCategoryResponse>();
        private IEnumerable<GetFileResponse>? _Files = new List<GetFileResponse>();
        private string _Icon_CSharp = "icons/c_sharp.png";
        private string _CategoryName { get; set; }
        private DateTime _CategoryCreateAt { get; set; }
        private MudTable<GetFileResponse> _table;
        private string? _SearchString = null;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            Dispatcher.Dispatch(new FilesCategoriesActions.LoadCategoriesAction());
            var result = await CategoriesClient.GetByIdAsync(Id).ConfigureAwait(false);
            
            _CategoryName     = result.Data.Name;
            _CategoryCreateAt = result.Data.CreatedAt;
            _Elements         = new List<GetCategoryResponse>() { result.Data };

            var filesResult = await FilesClient.GetAllAsync().ConfigureAwait(false);

            //TODO: Выяснить, что за категории содержатся/несодержатся
            // _Files = filesResult.Data.Where(x => x.Categories.Equals(result.Data.Name));
            _Files = filesResult.Data;
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

        private async Task OnFilesSearch(string text)
        {
            var result = await FilesClient.GetAllAsync().ConfigureAwait(false);
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
                
                _Files = data;
            }
            else
            {
                _Elements = null;
            }
        }
    }
}