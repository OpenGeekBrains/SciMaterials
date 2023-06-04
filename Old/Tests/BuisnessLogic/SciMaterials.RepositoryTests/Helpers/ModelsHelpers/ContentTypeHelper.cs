using SciMaterials.DAL.Resources.Contracts.Entities;

using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class ContentTypeHelper
{
    public static IEnumerable<ContentType> GetMany()
    {
        yield return GetOne();
    }

    public static ContentType GetOne()
    {
        return new ContentType
        {
            Id = Guid.NewGuid(),
            Name = "ContentTypeName",
            FileExtension = "FileExtension",

            Files =
            {
                new File { Id = Guid.NewGuid(), }
            },
        };
    }
}