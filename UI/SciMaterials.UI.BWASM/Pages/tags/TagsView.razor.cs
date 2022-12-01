using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ITagsClient TagsClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetTagResponse> _Elements = new List<GetTagResponse>();
        private string _Icon_HashTag = "icons/hash_tag.png";
        
        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            var result = await TagsClient.GetByIdAsync(Id);
            if (result.Succeeded)
            {
                _Elements = new List<GetTagResponse>() { result.Data };
            }
            else
            {
                _Elements = new GetTagResponse[] { };
            }
        }
        
        private void OnBackClick()
        {
            NavigationManager.NavigateTo("/tags_storage");
        }
    }
}