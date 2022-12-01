using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsStorage
    {
        [Inject] private ITagsClient TagsClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetTagResponse>? _Tags;
        
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            
            var result = await TagsClient.GetAllAsync();
            if (result.Succeeded)
            {
                var data = result.Data;
                _Tags = data.Take(15);
            }
            else
            {
                _Tags = new GetTagResponse[]{};
            }
        }
        
        private void OnTagClick(Guid Id)
        {
            NavigationManager.NavigateTo($"/tags_storage/{Id}");
        }
    }
}