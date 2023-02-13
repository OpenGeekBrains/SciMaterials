using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.WebApi.Clients.Tags;

public class TagsClient :
    ApiModifiedClientWithAddBase<Guid, GetTagResponse, AddTagRequest, EditTagRequest>,
    ITagsClient
{
    public TagsClient(HttpClient httpClient, ILogger<TagsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Tags;
}
