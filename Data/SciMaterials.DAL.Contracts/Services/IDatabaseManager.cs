namespace SciMaterials.DAL.Contracts.Services;

public interface IDatabaseManager
{
    /// <summary> Асинхронно удаляет базу данных, если она существует. </summary>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/></param>
    /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task DeleteDatabaseAsync(CancellationToken Cancel = default);

    /// <summary> Асинхронно инициализирует новую базу данных. </summary>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/> Значение по умолчанию: <value>default</value></param>
    /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task InitializeDatabaseAsync(CancellationToken Cancel = default);

    /// <summary> Асинхронно выполняет транзакцию заполнения таблиц базы данных тестовыми данными. В случае ошибки транзакция не выполняется.</summary>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/> Значение по умолчанию: <value>default</value></param>
    /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task SeedDatabaseAsync(CancellationToken Cancel = default);
}