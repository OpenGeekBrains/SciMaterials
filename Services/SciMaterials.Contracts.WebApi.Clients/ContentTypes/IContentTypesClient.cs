using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.Contracts.WebApi.Clients.ContentTypes;

public interface IContentTypesClient :
    IApiReadonlyClient<Guid,GetContentTypeResponse>,
    IApiModifyClient<Guid, AddContentTypeRequest, EditContentTypeRequest>
{
}