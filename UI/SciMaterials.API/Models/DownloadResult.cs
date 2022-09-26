namespace SciMaterials.API.Models;

public class DownloadResult
{
    public FileModel FileModel { get; set; } = null!;

    public Stream Data { get; set; } = null!;
}