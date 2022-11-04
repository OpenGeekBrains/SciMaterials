using SciMaterials.DAL.Models;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class RatingHelper
{
    public static IEnumerable<Rating> GetMany()
    {
        yield return GetOne();
    }

    public static Rating GetOne()
    {
        return new Rating
        {
            Id = Guid.NewGuid(),
            RatingValue = 5,

            User = new Author
            {
                Id = Guid.NewGuid(),
            },

            File =
            new File
            {
                Id = Guid.NewGuid(),
            },
            FileGroup = new FileGroup
            {
                Id = Guid.NewGuid(),
            },
        };
    }
}