namespace SciMaterials.Contracts.Result;

/// <summary>Возвращает результат операции для постраничного вывода</summary>
public class PageResult<TData> : Result
{
    private const int PageSizeDefault = 10;
    public List<TData>? Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = PageSizeDefault;
    public int TotalCount { get; set; }
    public int TotalPages => TotalCount / PageSize;

    public static new PageResult<TData> Success(List<TData> data, int totalCount = 0, int pageNumber = 1, int pageSize = PageSizeDefault) => new()
    {
        Data = data,
        TotalCount = totalCount == 0 ? data.Count : totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize,
    };

    public static new PageResult<TData> Error(string code, string message = "") => new() { Code = code, Message = message };

    public static new PageResult<TData> Error(Error error) => Error(error.Code, error.Message);

    public static implicit operator PageResult<TData>(List<TData> data) => Success(data);

    public static implicit operator PageResult<TData>((List<TData> Data, int TotalCount) t) 
        => Success(t.Data, t.TotalCount);

    public static implicit operator PageResult<TData>((List<TData> Data, int TotalCount, int PageNumber, int PageSize) t) 
        => Success(t.Data, t.TotalCount, t.PageNumber, t.PageSize);
}