using SciMaterials.Contracts.API.DTO.Urls;

namespace SciMaterials.Contracts.WebApi.Clients.Urls;

public interface IUrlsClient :
    IApiReadonlyClient<Guid, GetUrlResponse>,
    IApiModifyClient<Guid, AddUrlRequest, EditUrlRequest>
{
}