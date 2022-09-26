using System.Collections.Concurrent;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Models;

namespace SciMaterials.API.Data;

public class FileInfoMemoryRepository : IFileRepository
{
    private readonly ConcurrentDictionary<string, FileModel> _files = new ConcurrentDictionary<string, FileModel>();
    public bool Add(FileModel model)
    {
        return _files.TryAdd(model.Hash, model);
    }

    public FileModel? GetByHash(string hash)
    {
        if (_files.TryGetValue(hash, out var fileModel))
            return fileModel;

        return null;
    }
}