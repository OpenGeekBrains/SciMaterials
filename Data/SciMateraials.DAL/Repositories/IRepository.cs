namespace SciMaterials.Data.Repositories;

/// <summary> Интерфейс репозиториев. </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary> Получить экземпляр модели по Id. </summary>
    /// <param name="id"> Id экземпляра модли в БД. </param>
    /// <returns> Найденный в БД экземпляр модли или null. </returns>
    T GetById(Guid id);

    /// <summary> Добавить экзепляр модели в БД. </summary>
    /// <param name="entity"> Добавляемый экземпляр. </param>
    void Add(T entity);

    /// <summary> Обновить экземпляр модели. </summary>
    /// <param name="entity"> Экземпляр с новыми данными. </param>
    void Update(T entity);

    /// <summary> Удалить экземпляр модели из БД. </summary>
    /// <param name="id"> Id удаляемого экземпляра. </param>
    void Delete(Guid id);

    /// <summary> Получить список всех экзепляров из БД. </summary>
    /// <returns> Список экземпляров. </returns>
    List<T> GetAll();
}
