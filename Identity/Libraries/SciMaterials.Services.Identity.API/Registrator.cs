using Microsoft.Extensions.DependencyInjection;
using SciMaterials.Contracts.Identity.API;

namespace SciMaterials.Services.Identity.API;

public static class Registrator
{
	/// <summary>Метод расширения для Identity клиента</summary>
	/// <param name="services">Сервисы</param>
	/// <param name="serverUrl">Url сервера</param>
	/// <returns></returns>
	public static IServiceCollection AddIdentityClients(this IServiceCollection services, string serverUrl)
	{
		services.AddHttpClient<IIdentityApi, IdentityClient>("IdentityApi", c =>
		{
			c.BaseAddress = new Uri(serverUrl);
		});

		return services;
	}
}