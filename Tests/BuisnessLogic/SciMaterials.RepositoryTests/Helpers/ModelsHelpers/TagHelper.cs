using SciMaterials.DAL.Models;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal class TagHelper
{
    public static IEnumerable<Tag> GetMany()
    {
        yield return GetOne();
    }

    public static Tag GetOne()
    {
        return new Tag
        {
            Id = Guid.NewGuid(),
            Name = "FileGroupName",
            Files =
            {
                new File { Id = Guid.NewGuid(), }
            },
            FileGroups =
            {
                new FileGroup { Id = Guid.NewGuid(), }
            },
        };
    }
}