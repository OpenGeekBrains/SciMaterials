using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SciMaterials.DAL.Contexts;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

namespace SciMaterials.RepositoryTests.Helpers;

public class SciMateralsContextHelper
{
    public SciMaterialsContext Context { get; set; }

    public SciMateralsContextHelper()
    {
        var builder = new DbContextOptionsBuilder<SciMaterialsContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        var options = builder.Options;
        Context = new SciMaterialsContext(options);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        Context.AddRange(CategoryHelper.GetMany());
        Context.AddRange(AuthorHelper.GetMany());
        Context.AddRange(CommentHelper.GetMany());
        Context.AddRange(ContentTypeHelper.GetMany());
        Context.AddRange(FileGroupHelper.GetMany());
        //Context.AddRange(FileHelper.GetMany());
        Context.AddRange(TagHelper.GetMany());
        Context.AddRange(UrlHelper.GetMany());
        Context.AddRange(RatingHelper.GetMany());

        Context.SaveChanges();
    }
}