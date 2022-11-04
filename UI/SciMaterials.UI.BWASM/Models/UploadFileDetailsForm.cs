namespace SciMaterials.UI.BWASM.Models;

public class UploadFileDetailsForm
{
    public static readonly UploadFileDetailsForm Empty = new UploadFileDetailsForm();

    public Guid Id { get; init; }
    public string FileName { get; set; } = null!;
    public long Size { get; init; }
    public string ContentType { get; init; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}