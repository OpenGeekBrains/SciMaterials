using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.Contracts.API.Services.Comments;

public interface ICommentService : IApiService<Guid, GetCommentResponse>, IModifyService<AddCommentRequest, EditCommentRequest, Guid>
{
}