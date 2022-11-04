﻿using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class File : Resource
{
    public long Size { get; set; }
    public string? Hash { get; set; }
    public Guid? ContentTypeId { get; set; }
    public Guid? FileGroupId { get; set; }

    public ContentType? ContentType { get; set; }
    public FileGroup? FileGroup { get; set; }
}
