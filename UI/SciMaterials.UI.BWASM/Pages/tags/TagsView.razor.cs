using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ITagsClient TagsClient { get; set; }
        private IEnumerable<GetTagResponse> Elements = new List<GetTagResponse>();
        
        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            var result = await TagsClient.GetByIdAsync(Id);
            
            Elements = new List<GetTagResponse>() { result.Data };
        }
    }
}