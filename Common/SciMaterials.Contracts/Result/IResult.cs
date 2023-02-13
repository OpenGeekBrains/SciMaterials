namespace SciMaterials.Contracts.Result;

public interface IResult
{
    int Code { get; set; }
    bool Succeeded { get; set; }
    ICollection<string> Messages { get; set; }
}

public interface IResult<out TData> : IResult
{
    TData Data { get; }
}