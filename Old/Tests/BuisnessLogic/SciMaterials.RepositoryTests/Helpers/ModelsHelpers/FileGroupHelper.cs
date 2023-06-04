using SciMaterials.DAL.Resources.Contracts.Entities;
using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal class FileGroupHelper
{
    public static IEnumerable<FileGroup> GetMany()
    {
        yield return GetOne();
    }

    public static FileGroup GetOne()
    {
        return new FileGroup
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
            Files =
            {
                new File { Id = Guid.NewGuid(), }
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
            }
        };
    }
}