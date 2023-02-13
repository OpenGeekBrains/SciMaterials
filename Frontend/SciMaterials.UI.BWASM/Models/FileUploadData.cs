using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.Models;

public class FileUploadData
{
    public Guid Id { get; init; }
    public IBrowserFile File { get; init; } = null!;
    public string FileName { get; init; } = null!;
    
    public string ShortInfo { get; init; } = null!;
    public Guid Category { get; init; }

    public CancellationToken CancellationToken { get; init; }
    public Guid AuthorId { get; init; }
    public string? Description { get; init; }
}