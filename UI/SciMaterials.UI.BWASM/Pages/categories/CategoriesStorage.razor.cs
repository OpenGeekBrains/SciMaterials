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
        private IEnumerable<GetCategoryResponse> _data;
        private MudTable<GetCategoryResponse> _table;
        private string? _searchString;
        private Guid _searchingId;
    
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
                    case "id_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.Id);
                        break;
                    case "name_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.Name);
                        break;
                    case "description_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.Description);
                        break;
                    case "create_at_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.CreatedAt);
                        break;
                    case "parent_id_field" :
                        data = data.OrderByDirection(state.SortDirection, o => o.ParentId);
                        break;
                }
                
                _data = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
                return new TableData<GetCategoryResponse>() { TotalItems = result.Data.Count(), Items = _data };
            }
            
            return new TableData<GetCategoryResponse>() { TotalItems = 0, Items = null };
        }
    
        private async Task<TableData<GetCategoryResponse>> GetAll()
        {
            var result = await CategoriesClient.GetAllAsync();
            if (result.Succeeded)
            {
                var data = result.Data;
                _data = data.ToArray();
                
                _table.ReloadServerData();
                return new TableData<GetCategoryResponse>() { TotalItems = result.Data.Count(), Items = _data };
            }
            
            return new TableData<GetCategoryResponse>() { TotalItems = 0, Items = null };
        }
        
        // private async Task<TableData<GetCategoryResponse>> GetById()
        // {
        //     var result = await CategoriesClient.GetByIdAsync(_searchingId);
        //     if (result.Succeeded)
        //     {
        //         var data = result.Data;
        //         
        //         var list = new List<GetCategoryResponse>();
        //         list.Add(data);
        //
        //         _data = (IEnumerable<GetCategoryResponse>)list;
        //         
        //         _table.ReloadServerData();
        //         return new TableData<GetCategoryResponse>() { TotalItems = list.Count(), Items = _data };
        //     }
        //     
        //     return new TableData<GetCategoryResponse>() { TotalItems = 0, Items = null };
        // }
        
        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }
    }
}