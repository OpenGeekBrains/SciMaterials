using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.States.UploadFilesForm;

public record FileData(IBrowserFile BrowserFile)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; set; } = BrowserFile.Name;
    public string ContentType { get; init; } = BrowserFile.ContentType;
    public long Size { get; init; } = BrowserFile.Size;
}