using SciMaterials.DAL.Resources.Contracts.Entities;
using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

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
            Resource = new Resource
            {
                Id = Guid.NewGuid(),
            },
        };
    }
}