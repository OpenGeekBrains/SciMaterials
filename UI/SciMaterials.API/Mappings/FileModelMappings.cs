using SciMaterials.API.DTO;
using SciMaterials.API.Models;

namespace SciMaterials.API.Mappings;

public static class FileModelMappings
{
    public static FileUploadResponse? ToViewModel(this FileModel? model)
        => model is null
            ? null
            : new FileUploadResponse()
            {
                Hash = model.Hash,
                FileName = model.FileName,
                ContentType = model.ContentType,
                Size = model.Size
            };
}