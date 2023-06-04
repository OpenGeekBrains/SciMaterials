using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts.WebApi.Clients.Authors;

namespace SciMaterials.WebApi.Clients.Authors;

public class AuthorsClient :
    ApiModifiedClientWithAddBase<Guid, GetAuthorResponse, AddAuthorRequest, EditAuthorRequest>,
    IAuthorsClient
{
    public AuthorsClient(HttpClient httpClient, ILogger<AuthorsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Authors;
}
