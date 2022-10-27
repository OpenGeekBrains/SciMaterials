using NLog.Time;

using SciMaterials.DAL.Models;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal class FileHelper
{
    public static IEnumerable<File> GetMany()
    {
        yield return GetOne();
    }

    public static File GetOne()
    {
        return new File
        {
            Id = Guid.NewGuid(),
            Name = "FileGroupName",
            ShortInfo = "ShortInfo",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            Url = "Url",
            Size = long.MaxValue,
            Hash = "Hash",
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
            FileGroup = new FileGroup
            {
                Id = Guid.NewGuid(),
            },
            ContentType = new ContentType
            {
                Id = Guid.NewGuid(),
            },
        };
    }
}