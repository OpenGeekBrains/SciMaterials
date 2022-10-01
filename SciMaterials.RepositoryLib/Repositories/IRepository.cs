namespace SciMaterials.Data.Repositories;

/// <summary> Интерфейс репозиториев. </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary> Получить экземпляр модели по Id. </summary>
    /// <param name="id"> Id экземпляра модли в БД. </param>
    /// <param name="disableTracking"> Отключить отслеживание изменений. </param>
    /// <returns> Найденный в БД экземпляр модли или null. </returns>
    T GetById(Guid id, bool disableTracking = true);

    /// <summary> Получить экземпляр модели по Id. Асинхронный. </summary>
    /// <param name="id"> Id экземпляра модли в БД. </param>
    /// <param name="disableTracking"> Отключить отслеживание изменений. </param>
    /// <returns> Найденный в БД экземпляр модли или null. </returns>
    Task<T> GetByIdAsync(Guid id, bool disableTracking = true);

    /// <summary> Добавить экзепляр модели в БД. </summary>
    /// <param name="entity"> Добавляемый экземпляр. </param>
    void Add(T entity);

    /// <summary> Добавить экзепляр модели в БД. Асинхронный. </summary>
    /// <param name="entity"> Добавляемый экземпляр. </param>
    void AddAsync(T entity);

    /// <summary> Обновить экземпляр модели. </summary>
    /// <param name="entity"> Экземпляр с новыми данными. </param>
    void Update(T entity);

    /// <summary> Обновить экземпляр модели. Асинхронный. </summary>
    /// <param name="entity"> Экземпляр с новыми данными. </param>
    void UpdateAsync(T entity);

    /// <summary> Удалить экземпляр модели из БД. </summary>
    /// <param name="id"> Id удаляемого экземпляра. </param>
    void Delete(Guid id);

    /// <summary> Удалить экземпляр модели из БД. Асинхронный. </summary>
    /// <param name="id"> Id удаляемого экземпляра. </param>
    void DeleteAsync(Guid id);

    /// <summary> Получить список всех экзепляров из БД. </summary>
    /// <param name="disableTracking"> Отключить отслеживание изменений. </param>
    /// <returns> Список экземпляров. </returns>
    List<T> GetAll(bool disableTracking = true);

    /// <summary> Получить список всех экзепляров из БД. Асинхронный. </summary>
    /// <param name="disableTracking"> Отключить отслеживание изменений. </param>
    /// <returns> Список экземпляров. </returns>
    Task<List<T>> GetAllAsync(bool disableTracking = true);
}
