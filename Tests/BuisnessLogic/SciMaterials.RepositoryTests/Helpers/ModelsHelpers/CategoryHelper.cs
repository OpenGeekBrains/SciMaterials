using SciMaterials.DAL.Models;
using SciMaterials.DAL.Models.Base;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class CategoryHelper
{
    public static IEnumerable<Category> GetMany()
    {
        yield return GetOne();
    }

    public static Category GetOne()
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Description = "Category description",
            Name = "CategoryName",
            Resources = new HashSet<Resource>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                },
            }
        };
    }
}