namespace SciMaterials.API.Models;

public readonly struct FileSaveResult
{
    public string Hash { get; }
    public long Size { get; }

    public FileSaveResult(string hash, long size)
        => (Hash, Size) = (hash, size);
}