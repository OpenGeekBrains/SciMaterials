using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.Contracts.WebApi.Clients.Comments;

namespace SciMaterials.WebApi.Clients.Comments;

public class CommentsClient :
    ApiModifiedClientWithAddBase<Guid, GetCommentResponse, AddCommentRequest, EditCommentRequest>,
    ICommentsClient
{
    public CommentsClient(HttpClient httpClient, ILogger<CommentsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Comments;
}
