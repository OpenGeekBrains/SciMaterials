using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Helpers;

public class SciMateralsContextHelper
{
    public static SciMaterialsContext Create()
    {
        var options = new DbContextOptionsBuilder<SciMaterialsContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
           .Options;

        var context = new SciMaterialsContext(options);

        //context.Database.EnsureDeleted();
        //context.Database.EnsureCreated();

        context.AddRange(CategoryHelper.GetMany());
        context.AddRange(AuthorHelper.GetMany());
        context.AddRange(ContentTypeHelper.GetMany());

        context.SaveChanges();

        return context;
    }
}