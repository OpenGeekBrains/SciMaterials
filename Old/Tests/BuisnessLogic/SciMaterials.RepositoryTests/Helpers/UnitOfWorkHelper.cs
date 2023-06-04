using Moq;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.Contracts.Entities;
using SciMaterials.DAL.Resources.Repositories.Files;
using SciMaterials.DAL.Resources.Repositories.Users;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.DAL.Resources.Repositories.Ratings;
using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

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

        var comment_reposiroty_logger = new Mock<ILogger<CommentRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Comment>())
            .Returns(new CommentRepository(context, comment_reposiroty_logger.Object));

        var content_reposiroty_logger = new Mock<ILogger<ContentTypeRepository>>();
        unit_of_work.Setup(x => x.GetRepository<ContentType>())
            .Returns(new ContentTypeRepository(context, content_reposiroty_logger.Object));

        var fileGroup_reposiroty_logger = new Mock<ILogger<FileGroupRepository>>();
        unit_of_work.Setup(x => x.GetRepository<FileGroup>())
            .Returns(new FileGroupRepository(context, fileGroup_reposiroty_logger.Object));

        var url_reposiroty_logger = new Mock<ILogger<UrlRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Url>())
            .Returns(new UrlRepository(context, url_reposiroty_logger.Object));

        var rating_reposiroty_logger = new Mock<ILogger<RatingRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Rating>())
            .Returns(new RatingRepository(context, rating_reposiroty_logger.Object));

        var tag_reposiroty_logger = new Mock<ILogger<TagRepository>>();
        unit_of_work.Setup(x => x.GetRepository<Tag>())
            .Returns(new TagRepository(context, tag_reposiroty_logger.Object));

        return unit_of_work;
    }
}
