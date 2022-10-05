using System.Collections.Concurrent;
using SciMaterials.FilesAPI.Data.Interfaces;
using SciMaterials.FilesAPI.Models;

namespace SciMaterials.FilesAPI.Data;

public class FileInfoMemoryRepository : IFileRepository<Guid>
{
    private readonly ConcurrentDictionary<Guid, FileModel> _files = new();
    public bool Add(FileModel model)
        => _files.TryAdd(model.Id, model);

    public void Update(FileModel model)
        => _files[model.Id] = model;

    public void AddOrUpdate(FileModel model)
    {
        _files.AddOrUpdate(
            model.Id,
            model,
            (id, fileInfo) => fileInfo = model);
    }

    public void Delete(Guid id)
        => _files.Remove(id, out _);

    public FileModel? GetByHash(string hash)
        => _files.Values.SingleOrDefault(item => item.Hash == hash);

    public FileModel? GetById(Guid id)
     => _files.GetValueOrDefault(id);

    public FileModel? GetByName(string fileName)
        => _files.Values.SingleOrDefault(item => item.FileName == fileName);
}