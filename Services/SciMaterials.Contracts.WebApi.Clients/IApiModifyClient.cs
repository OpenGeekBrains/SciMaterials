namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiModifyClient<TId, TAddRequest, TEditRequest> :
    IApiAddClient<TId, TAddRequest>,
    IApiEditClient<TId,TEditRequest>,
    IApiDeleteClient<TId>
{ }
