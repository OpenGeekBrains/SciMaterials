using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Services;

public abstract class ServiceBase
{
    protected readonly ILogger<ServiceBase> _Logger;

    protected ServiceBase(ILogger<ServiceBase> Logger) => _Logger = Logger;

    protected Result<TData> LoggedError<TData>(Error Error, Exception exception, string MessagePattern, params object?[] Parameters)
    {
        _Logger.LogError(exception, MessagePattern, Parameters);
        return Result<TData>.Failure(Error);
    }

    protected Result<TData> LoggedError<TData>(Error Error, string MessagePattern, params object?[] Parameters)
    {
        _Logger.LogError(MessagePattern, Parameters);
        return Result<TData>.Failure(Error);
    }
}