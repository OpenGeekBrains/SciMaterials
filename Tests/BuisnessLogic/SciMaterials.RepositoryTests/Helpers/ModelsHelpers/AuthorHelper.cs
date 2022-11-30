using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class AuthorHelper
{
    public static IEnumerable<Author> GetMany()
    {
        yield return GetOne();
    }

    public static Author GetOne()
    {
        return new Author
        {
            Id = Guid.NewGuid(),
            Name = "AuthorName",
            Surname = "AuthorSureName",
            Email = "AuthorEmail",
            Phone = "+7 (123) 456-78-90",
            User = new User
            {
                Id= Guid.NewGuid(),
            },
            Resources =
            {
                new Resource { Id = Guid.NewGuid(), }
            },
            Comments =
            {
                new Comment { Id = Guid.NewGuid(), }
            },
            Ratings =
            {
                new Rating {Id = Guid.NewGuid(), }
            }
        };
    }
}