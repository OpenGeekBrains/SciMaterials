namespace SciMaterials.API.DTO;

public class FileUploadResponse
{
    public string Hash { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

}