using SciMaterials.API.Models;

namespace SciMaterials.API.Data.Interfaces;

public interface IFileRepository<T>
{
    bool Add(FileModel model);
    void Update(FileModel model);
    void AddOrUpdate(FileModel model);
    void Delete(T id);
    FileModel? GetByHash(string hash);
    FileModel? GetById(T id);
    FileModel? GetByName(string name);
}
