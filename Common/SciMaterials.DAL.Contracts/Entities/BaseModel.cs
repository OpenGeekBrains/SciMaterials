namespace SciMaterials.DAL.Contracts.Entities;

public class BaseModel
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}