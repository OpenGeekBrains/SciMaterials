using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.Contracts.WebApi.Clients.Comments;

public interface ICommentsClient :
    IApiReadonlyClient<Guid, GetCommentResponse>,
    IApiModifyClient<Guid, AddCommentRequest, EditCommentRequest>,
    IApiDeleteClient<Guid>
{
}