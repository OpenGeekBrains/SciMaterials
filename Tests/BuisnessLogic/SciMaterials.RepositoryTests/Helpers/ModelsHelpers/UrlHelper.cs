using SciMaterials.DAL.Models;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal class UrlHelper
{
    public static IEnumerable<Url> GetMany()
    {
        yield return GetOne();
    }

    public static Url GetOne()
    {
        return new Url
        {
            Id = Guid.NewGuid(),
            Name = "FileGroupName",
            ShortInfo = "ShortInfo",
            Description = "Description",
           
            CreatedAt = DateTime.UtcNow,
            Author = new Author
            {
                Id = Guid.NewGuid(),
            },
            Comments =
            {
                new Comment { Id = Guid.NewGuid(), }
            },
            Tags =
            {
                new Tag {Id = Guid.NewGuid(), }
            },
            Categories =
            {
                new Category {Id = Guid.NewGuid(), }
            },
            Ratings =
            {
                new Rating {Id = Guid.NewGuid(), }
            },
            Link = "Link",
        };
    }
}