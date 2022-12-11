using SciMaterials.DAL.Resources.Contracts.Entities;

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
            Comments = new List<Comment>
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                }
            },
            Tags = new List<Tag>
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                }
            },
            Categories = new List<Category>
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                }
            },
            Ratings = new List<Rating>
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                }
            },
            Link = "Link",
        };
    }
}
