using AutoMapper;

using Microsoft.Extensions.Logging;

using Moq;

using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.RepositoryLib.Repositories;
using SciMaterials.Services.API.Services.Files;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Services.API.Tests.Services.Files;

public class FileServiceTests
{


    [Fact]
    public async Task GetByIdAsync_Return_Exists_Data()
    {
        #region Arrange

        var expected_file_id = Guid.NewGuid();

        var api_settings_mock = new Mock<IApiSettings>();
        var file_store_mock = new Mock<IFileStore>();
        var db_mock = new Mock<IUnitOfWork<SciMaterialsContext>>();
        var linkReplace_mock = new Mock<ILinkReplaceService>();
        var linkShortCut_mock = new Mock<ILinkShortCutService>();
        var mapper_mock = new Mock<IMapper>();
        var logger_mock = new Mock<ILogger<FileService>>();

        var file_repository = new Mock<IRepository<File>>();

        file_repository
           .Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
           .ReturnsAsync(new File
           {
               Id = expected_file_id
           });

        mapper_mock
           .Setup(mapper => mapper.Map<GetFileResponse>(It.IsAny<File>()))
           .Returns<File>(file => new GetFileResponse
           {
               Id = file.Id,
           });

        api_settings_mock.Setup(s => s.BasePath).Returns("path");

        db_mock.Setup(s => s.GetRepository<File>()).Returns(file_repository.Object);

        var service = new FileService(
            api_settings_mock.Object,
            file_store_mock.Object,
            linkReplace_mock.Object,
            linkShortCut_mock.Object,
            db_mock.Object,
            mapper_mock.Object,
            logger_mock.Object);

        #endregion

        #region Act

        var result = await service.GetByIdAsync(expected_file_id, false);

        #endregion

        #region Assert

        Assert.NotNull(result);
        Assert.NotNull(result.Data);

        var actual_file_id = result.Data.Id;

        Assert.Equal(expected_file_id, actual_file_id);

        #endregion

        #region Assert Moq

        api_settings_mock.Verify(s => s.BasePath);
        api_settings_mock.Verify(s => s.Separator);
        mapper_mock.Verify(mapper => mapper.Map<GetFileResponse>(It.Is<File>(f => f.Id == expected_file_id)));
        file_repository.Verify(r => r.GetByIdAsync(It.Is<Guid>(guid => guid == expected_file_id), It.IsAny<bool>(), It.IsAny<bool>()));
        db_mock.Verify(d => d.GetRepository<File>());

        api_settings_mock.VerifyNoOtherCalls();
        mapper_mock.VerifyNoOtherCalls();
        file_repository.VerifyNoOtherCalls();
        db_mock.VerifyNoOtherCalls();

        #endregion
    }
}
