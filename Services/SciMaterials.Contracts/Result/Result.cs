namespace SciMaterials.Contracts.Result;

/// <summary>Возвращает результат операции</summary>
public class Result
{
    /// <summary> Код результата. </summary>
    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    /// <summary> Успешность выполнения операции (true - успешно | false - ошибка) </summary>
    public bool Succeeded => Code.Length == 0;

    /// <summary> Неуспешное выполнения операции (true - ошибка | false - успешно) </summary>
    public bool IsFaulted => !Succeeded;

    /// <summary> Возращает успешный результат выполнения операции. </summary>
    /// <returns> Результат операции. </returns>
    public static Result Success() => new();

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Failure(string code) => new() { Code = code };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="message"> Дополнительно сообщение об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Failure(string code, string message) => new() { Code = code, Message = message };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="error"> Объект ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Failure(Error error) => Failure(error.Code, error.Message);

    public Task<Result> ToTask() => Task.FromResult(this);
}