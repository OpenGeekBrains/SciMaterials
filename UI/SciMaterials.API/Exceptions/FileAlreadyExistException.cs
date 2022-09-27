namespace SciMaterials.API.Exceptions;

public class FileAlreadyExistException : Exception
{
    public FileAlreadyExistException(string fileName) : base($"File {fileName} already exist")
    {
    }
}