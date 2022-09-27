namespace SciMaterials.API.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string? Hash { get; set; } = string.Empty;

    }
}