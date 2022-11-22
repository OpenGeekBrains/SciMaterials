﻿using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Category : NamedModel
{
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category>? Children { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ResourceId { get; set; }
    public ICollection<Resource>? Resources { get; set; }
}
