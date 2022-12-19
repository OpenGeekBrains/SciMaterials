using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Contracts.WebApi.Clients.Tags;

public interface ITagsClient :
    IApiReadonlyClient<Guid, GetTagResponse>,
    IApiModifyClient<Guid, AddTagRequest, EditTagRequest>,
    IApiDeleteClient<Guid>
{
}