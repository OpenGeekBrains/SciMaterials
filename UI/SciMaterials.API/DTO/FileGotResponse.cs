namespace SciMaterials.FilesAPI.DTO;

public class FileGotResponse : ResponseFileModel
{
    public string Hash { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long Size { get; set; }

}
