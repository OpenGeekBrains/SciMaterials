namespace SciMaterials.Contracts.API.DTO.Comments;

public class GetCommentResponse
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }
}