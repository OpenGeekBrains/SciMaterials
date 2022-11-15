using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Services;

public abstract class ServiceBase
{
    protected readonly ILogger _logger;

    protected ServiceBase(ILogger logger)
    {
        _logger = logger;
    }

    protected Result<TData> LoggedError<TData>(Error error, Exception ex, string messagePattern, params object?[] args)
    {
        _logger.LogError(ex, messagePattern, args);
        return Result<TData>.Failure(error);
    }

    protected Result<TData> LoggedError<TData>(Error error, string messagePattern, params object?[] args)
    {
        _logger.LogError(messagePattern, args);
        return Result<TData>.Failure(error);
    }
}