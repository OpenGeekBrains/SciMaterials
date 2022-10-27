using SciMaterials.DAL.Models;

using File = SciMaterials.DAL.Models.File;

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
            Author = new Author
            {
                Id = Guid.NewGuid(),
            },
            CreatedAt = DateTime.UtcNow,
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
