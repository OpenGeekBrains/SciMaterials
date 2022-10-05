namespace SciMaterials.FilesAPI.DTO;

public class FileUploadResponse : ResponseFileModel
{
    public string Hash { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

}
