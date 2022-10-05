namespace SciMaterials.FilesAPI.DTO;

public class FileNotFoundResponse : ResponseFileModel
{
    public string ErrorMessage { get; set; } = "File wasn't found on the server";

}
