using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.Contracts.API.Services.Authors;

public interface IAuthorService : IApiService<Guid, GetAuthorResponse>, IModifyService<AddAuthorRequest, EditAuthorRequest, Guid>
{
}