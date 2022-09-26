namespace SciMaterials.API.Models
{
    public class FileModel
    {
        public string Hash { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string RandomFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}