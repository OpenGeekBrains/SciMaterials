using SciMaterials.DAL.Resources.Contracts.Entities;

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
            Resources = new List<Resource>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                },
            },
        };
    }
}