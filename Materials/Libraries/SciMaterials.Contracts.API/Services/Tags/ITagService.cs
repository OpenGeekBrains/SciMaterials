using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Contracts.API.Services.Tags;

public interface ITagService : IApiService<Guid, GetTagResponse>, IModifyService<AddTagRequest, EditTagRequest, Guid>
{
}