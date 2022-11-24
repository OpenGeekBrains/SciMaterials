using Microsoft.AspNetCore.Components;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsStorage
    {
        [Inject] private ITagsClient TagsClient { get; set; }

        // private async Task<GetTagResponse> GetAll()
        // {
        //     var result = await TagsClient.GetAllAsync();
        //     if (result.Succeeded)
        //     {
        //         return result.Data;
        //     }
        //
        //     return new GetTagResponse() { };
        // }
    }
}