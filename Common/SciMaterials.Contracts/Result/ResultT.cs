namespace SciMaterials.Contracts.Result;
public class Result<TData> : Result
{
    public TData? Data { get; init; } = default!;

    public static new Result<TData> Success() => new();

    public static Result<TData> Success(TData data) => new() { Data = data };

    public static new Result<TData> Failure(Result result) => new() { Code = result.Code, Message = result.Message };

    public static new Result<TData> Failure(Error error) => new() { Code = error.Code, Message = error.Message };

    public static new Result<TData> Failure<TError>(Result<TError> result) => new() { Code = result.Code, Message = result.Message };

    public new Task<Result<TData>> ToTask() => Task.FromResult(this);

    public static implicit operator Result<TData>(TData data) => Success(data);
}
