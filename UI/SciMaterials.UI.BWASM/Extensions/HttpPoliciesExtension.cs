using Polly;
using Polly.Extensions.Http;

namespace SciMaterials.UI.BWASM.Extensions;

public static class HttpPoliciesExtension
{
    /// <summary>Метод расширения для политики повтора запросов для http клиентов</summary>
    /// <returns></returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
           .HandleTransientHttpError()
           .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
           .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}
