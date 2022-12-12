using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsStorage
    {
        [Inject] private ITagsClient TagsClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetTagResponse>? _Tags = new List<GetTagResponse>();
        private string? _Icon_HashTag = "icons/hash_tag.png";
        private bool _IsLoading = true;

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await TagsClient.GetAllAsync().ConfigureAwait(false);
            if (result.Succeeded)
            {
                _Tags      = result.Data;
                _IsLoading = false;
            }
        }
        
        private async Task OnSearchAsync(string text)
        {
            var result = await TagsClient.GetAllAsync().ConfigureAwait(false);
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
        
        private void OnTagClick(Guid Id)
        {
            NavigationManager.NavigateTo($"/tags_storage/{Id}");
        }
        
        private async Task OnRefreshClickAsync()
        {
            await OnInitializedAsync();
        }
    }
}