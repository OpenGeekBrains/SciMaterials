namespace SciMaterials.Contracts.API.DTO.Comments;

public class AddCommentRequest
{
    public Guid? ParentId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; } = null!;
    public Guid ResourceId { get; set; }
}