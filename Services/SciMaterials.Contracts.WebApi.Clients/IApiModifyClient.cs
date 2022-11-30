namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiModifyClient<TId, in TAddRequest, in TEditRequest> :
    IApiAddClient<TId, TAddRequest>,
    IApiEditClient<TId,TEditRequest>,
    IApiDeleteClient<TId>
{ }
