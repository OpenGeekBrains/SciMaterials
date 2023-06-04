using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.States.FilesUploadHistory;

public record FileUploadState(IBrowserFile BrowserFile, string CategoryName = "", Guid CategoryId = default)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = BrowserFile.Name;
    public long Size { get; init; } = BrowserFile.Size;
    public IBrowserFile BrowserFile { get; init; } = BrowserFile;
    public string CategoryName { get; init; } = CategoryName;
    public Guid CategoryId { get; init; } = CategoryId;
    public string AuthorName { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentType { get; init; } = BrowserFile.ContentType;
    public string? Description { get; init; }
    public UploadState State { get; init; } = UploadState.Scheduled;
    public CancellationTokenSource? CancellationSource { get; init; }
}