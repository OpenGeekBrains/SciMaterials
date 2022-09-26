using SciMaterials.API.Models;

namespace SciMaterials.API.Data.Interfaces;

public interface IFileRepository
{
    bool Add(FileModel model);
    FileModel? GetByHash(string hash);
}
