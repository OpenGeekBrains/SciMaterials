using SciMaterials.Contracts.API.DTO.Urls;

namespace SciMaterials.Contracts.API.Services.Urls;

public interface IUrlService : IApiService<Guid, GetUrlResponse>, IModifyService<AddUrlRequest, EditUrlRequest, Guid>
{
}