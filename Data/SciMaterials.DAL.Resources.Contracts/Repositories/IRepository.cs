namespace SciMaterials.DAL.Resources.Contracts.Repositories;

/// <summary>Интерфейс репозиториев</summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Получить кол-во экземпляров в БД</summary>
    /// <returns> Кол-во экземпляров</returns>
    int GetCount();

    /// <summary>Получить кол-во экземпляров в БД. Асинхронный</summary>
    /// <returns> Кол-во экземпляров</returns>
    Task<int> GetCountAsync();

    /// <summary>Получить страницу экземпляров</summary>
    /// <param name="PageNumber"> Номер страницы</param>
    /// <param name="PageSize"> Размер страницы</param>
    /// <returns> Список экземпляров</returns>
    List<T> GetPage(int PageNumber, int PageSize);

    /// <summary>Получить страницу экземпляров. Асинхронный</summary>
    /// <param name="PageNumber"> Номер страницы</param>
    /// <param name="PageSize"> Размер страницы</param>
    /// <returns> Список экземпляров</returns>
    Task<List<T>> GetPageAsync(int PageNumber, int PageSize);

    /// <summary>Получить экземпляр модели по Id</summary>
    /// <param name="Id"> Id экземпляра модли в БД</param>
    /// <returns> Найденный в БД экземпляр модли или null</returns>
    T? GetById(Guid Id);

    /// <summary>Получить экземпляр модели по Id. Асинхронный</summary>
    /// <param name="Id"> Id экземпляра модли в БД</param>
    /// <returns> Найденный в БД экземпляр модли или null</returns>
    Task<T?> GetByIdAsync(Guid Id);

    /// <summary>Добавить экзепляр модели в БД</summary>
    /// <param name="entity"> Добавляемый экземпляр</param>
    void Add(T entity);

    /// <summary>Добавить экзепляр модели в БД. Асинхронный</summary>
    /// <param name="entity"> Добавляемый экземпляр</param>
    Task AddAsync(T entity);

    /// <summary>Обновить экземпляр модели</summary>
    /// <param name="entity"> Экземпляр с новыми данными</param>
    void Update(T entity);

    /// <summary>Обновить экземпляр модели. Асинхронный</summary>
    /// <param name="entity"> Экземпляр с новыми данными</param>
    Task UpdateAsync(T entity);

    /// <summary>Удалить экземпляр модели из БД</summary>
    /// <param name="entity"> Id удаляемого экземпляра</param>
    void Delete(T entity);

    /// <summary>Удалить экземпляр модели из БД. Асинхронный</summary>
    /// <param name="entity"> Id удаляемого экземпляра</param>
    Task DeleteAsync(T entity);

    /// <summary>Удалить экземпляр модели из БД</summary>
    /// <param name="Id"> Id удаляемого экземпляра</param>
    void Delete(Guid Id);

    /// <summary>Удалить экземпляр модели из БД. Асинхронный</summary>
    /// <param name="Id"> Id удаляемого экземпляра</param>
    Task DeleteAsync(Guid Id);

    /// <summary>Получить список всех экзепляров из БД</summary>
    /// <returns> Список экземпляров</returns>
    List<T> GetAll();

    /// <summary>Получить список всех экзепляров из БД. Асинхронный</summary>
    /// <returns> Список экземпляров</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>Получить экземпляр по хэш-коду. Асинхронный</summary>
    /// <param name="Hash"> Искомый хэш-код (в текущем методе договорено о строчном варианте хэша)</param>
    /// <returns> Экземпляр класса или null</returns>
    Task<T?> GetByHashAsync(string Hash);

    /// <summary>Получить экземпляр по хэш-коду</summary>
    /// <param name="Hash"> Искомый хэш-код (в текущем методе договорено о строчном варианте хэша)</param>
    /// <returns> Экземпляр класса или null</returns>
    T? GetByHash(string Hash);

    /// <summary>Получить экземпляр по наименованию. Асинхронный</summary>
    /// <param name="Name"> Искомое наименование</param>
    /// <returns> Экземпляр класса или null</returns>
    Task<T?> GetByNameAsync(string Name);

    /// <summary>Получить экземпляр по наименованию</summary>
    /// <param name="Name"> Искомое наименование</param>
    /// <returns> Экземпляр класса или null</returns>
    T? GetByName(string Name);
}
