using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.Contracts.API.Services.ContentTypes;

public interface IContentTypeService : IApiService<Guid, GetContentTypeResponse>, IModifyService<AddContentTypeRequest, EditContentTypeRequest, Guid>
{
}