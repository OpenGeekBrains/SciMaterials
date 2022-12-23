using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsStorage
    {
        [Inject] private ITagsClient _TagsClient { get; set; }
        [Inject] private NavigationManager _NavigationManager { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }
        private IEnumerable<GetTagResponse>? _Tags = new List<GetTagResponse>();
        private string? _Icon_HashTag = "icons/hash_tag.png";
        private bool _IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await _TagsClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                _Tags      = result.Data;
                _IsLoading = false;
            }
        }
        
        private async Task OnSearchAsync(string text)
        {
            var result = await _TagsClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                var data = result.Data;
                data = data.Where(element =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return true;
                    if (element.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
                        return true;

                    return false;
                }).ToArray();
                
                _Tags = data;
            }
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
            
            await _DialogService.ShowAsync<TagsAddDialog>(
                title: "Add new tag",
                options: options).ConfigureAwait(false);
        }
        
        private void OnTagClick(Guid TagId)
        {
            _NavigationManager.NavigateTo($"/tags_storage/{TagId}");
        }
        
        private async Task OnRefreshClickAsync()
        {
            await OnInitializedAsync();
        }
    }
}