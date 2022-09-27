using System.Collections.Concurrent;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Models;

namespace SciMaterials.API.Data;

public class FileInfoMemoryRepository : IFileRepository<Guid>
{
    private readonly ConcurrentDictionary<Guid, FileModel> _files = new();
    public bool Add(FileModel model)
        => _files.TryAdd(model.Id, model);

    public void Update(FileModel model)
        => _files[model.Id] = model;

    public void Delete(Guid id)
        => _files.Remove(id, out _);

    public FileModel? GetByHash(string hash)
        => _files.Values.SingleOrDefault(item => item.Hash == hash);

    public FileModel? GetById(Guid id)
     => _files.GetValueOrDefault(id);

    public FileModel? GetByName(string fileName)
        => _files.Values.SingleOrDefault(item => item.FileName == fileName);
}