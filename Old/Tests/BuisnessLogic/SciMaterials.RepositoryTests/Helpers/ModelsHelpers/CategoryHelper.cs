using SciMaterials.DAL.Resources.Contracts.Entities;

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