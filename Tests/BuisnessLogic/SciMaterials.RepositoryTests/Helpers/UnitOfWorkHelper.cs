using Moq;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Data.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories.FilesRepositories;
using SciMaterials.RepositoryLib.Repositories.UsersRepositories;
using File = SciMaterials.DAL.Models.File;
using SciMaterials.RepositoryTests.Tests;
using SciMaterials.RepositoryTests.Helpers.ModelsHelpers;
using SciMaterials.RepositoryLib.Repositories.UrlsRepositories;
using SciMaterials.RepositoryLib.Repositories.RatingRepositories;

namespace SciMaterials.RepositoryTests.Helpers;

public static class UnitOfWorkHelper
{
    public static Mock<IUnitOfWork<SciMaterialsContext>> GetUnitOfWorkMock()
    {
        var context = new SciMateralsContextHelper().Context;
        ILoggerFactory loggerFactory = new LoggerFactory();
        var logger = new Logger<UnitOfWork<SciMaterialsContext>>(loggerFactory);

        var unitOfWork = new Mock<IUnitOfWork<SciMaterialsContext>>();
        unitOfWork.Setup(x => x.GetRepository<Category>()).Returns(new CategoryRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Author>()).Returns(new AuthorRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<File>()).Returns(new FileRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Comment>()).Returns(new CommentRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<ContentType>()).Returns(new ContentTypeRepository(context, logger));
        //unitOfWork.Setup(x => x.GetRepository<FileGroup>()).Returns(new FileGroupRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Url>()).Returns(new UrlRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Rating>()).Returns(new RatingRepository(context, logger));
        unitOfWork.Setup(x => x.GetRepository<Tag>()).Returns(new TagRepository(context, logger));

        return unitOfWork;
    }
}