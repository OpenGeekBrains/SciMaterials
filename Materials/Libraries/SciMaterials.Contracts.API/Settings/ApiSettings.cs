namespace SciMaterials.Contracts.API.Settings;

public class ApiSettings
{
    public const string SectionName = "ApiSettings";
    public virtual string BasePath { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public virtual string Separator { get; set; } = ",";
}