using Moq;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.UnitOfWork;

namespace SciMaterials.RepositoryTests.Helpers;

public static class UnitOfWorkHelper
{
    public static Mock<IUnitOfWork<SciMaterialsContext>> GetUnitOfWorkMock()
    {
        var context = SciMateralsContextHelper.Create();

        var unit_of_work = new Mock<IUnitOfWork<SciMaterialsContext>>();

        var category_reposiroty_logger = new Mock<ILogger<CategoryRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Category>())
           .Returns(new CategoryRepository(context, category_reposiroty_logger.Object));

        var author_reposiroty_logger = new Mock<ILogger<AuthorRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Author>())
           .Returns(new AuthorRepository(context, author_reposiroty_logger.Object));

        return unit_of_work;
    }
}
