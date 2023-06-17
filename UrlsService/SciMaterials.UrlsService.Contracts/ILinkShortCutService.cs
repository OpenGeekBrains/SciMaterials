using SciMaterials.Contracts.Result;

namespace SciMaterials.UrlsService.Contracts;

public interface ILinkShortCutService
{
	Task<Result<string>> AddAsync(string sourceAddress, CancellationToken Cancel = default);
	Task<Result<string>> GetAsync(string sourceAddress, bool isRedirect = false, CancellationToken Cancel = default);
	string GetLinkBasePath();
}