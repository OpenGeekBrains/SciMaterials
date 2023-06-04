using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Materials.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(context, e);
        }
    }

    private async Task HandleErrorAsync(HttpContext Context, Exception exception)
    {
        Context.Response.Clear();
        _logger.LogError(exception, exception.Message);
        var result = Result.Failure(Errors.App.Unhandled);
        await Context.Response.WriteAsJsonAsync(result);
    }
}
