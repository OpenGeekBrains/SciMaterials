namespace SciMaterials.API.Exceptions;

public class FileAlreadyExistException : Exception
{
    public string FileName { get; }

    public FileAlreadyExistException(string fileName) : this(fileName, $"File {fileName} already exist") { }
    public FileAlreadyExistException(string fileName, string message) : base(message)
       => FileName = fileName;
}
