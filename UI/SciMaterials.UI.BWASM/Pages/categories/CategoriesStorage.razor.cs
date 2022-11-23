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
        private MudTable<GetCategoryResponse> _mudTable;
        private List<string> clickedEvents = new();
        private IEnumerable<GetCategoryResponse> Elements = new List<GetCategoryResponse>();
        private int _selectedRowNumber = -1;
        private string? _searchString;

        private async Task<TableData<GetCategoryResponse>> ServerLoadData(TableState state)
        {
            var result = await CategoriesClient.GetAllAsync();
            if (result.Succeeded)
            {
                var data = result.Data;
                data = data.Where(element =>
                {
                    if (string.IsNullOrWhiteSpace(_searchString))
                        return true;
                    if (element.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (element.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
    
                    return false;
                }).ToArray();
                
                switch (state.SortLabel)
                {
                    case "name_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.Name);
                        break;
                    case "description_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.Description);
                        break;
                    case "create_at_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.CreatedAt);
                        break;
                }
                
                Elements = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
                return new TableData<GetCategoryResponse>() { TotalItems = result.Data.Count(), Items = Elements };
            }
            
            return new TableData<GetCategoryResponse>() { TotalItems = 0, Items = null };
        }
        
        private void RowClickEvent(TableRowClickEventArgs<GetCategoryResponse> tableRowClickEventArgs)
        {
            var categoryId = tableRowClickEventArgs.Item.Id;
            NavigationManager.NavigateTo($"/categories_storage/{categoryId}");
        }
        
        private string SelectedRowClassFunc(GetCategoryResponse element, int rowNumber)
        {
            if (_selectedRowNumber == rowNumber)
            {
                _selectedRowNumber = -1;
                clickedEvents.Add("Selected Row: None");
                return string.Empty;
            }
            else if (_mudTable.SelectedItem != null && _mudTable.SelectedItem.Equals(element))
            {
                _selectedRowNumber = rowNumber;
                clickedEvents.Add($"Selected Row: {rowNumber}");
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        
        private void OnSearch(string text)
        {
            _searchString = text;
            _mudTable.ReloadServerData();
        }
    }
}