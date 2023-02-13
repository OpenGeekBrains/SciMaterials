using SciMaterials.DAL.Resources.Contracts.Entities;

using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class CommentHelper
{
    public static IEnumerable<Comment> GetMany()
    {
        yield return GetOne();
    }

    public static Comment GetOne()
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            ParentId = Guid.NewGuid(),
            Text = "Some text",
            CreatedAt = DateTime.UtcNow,

            Author = new Author
            {
                Id = Guid.NewGuid(),
            },

            Resource =
            new File
            {
                Id = Guid.NewGuid(),
            },
        };
    }
}