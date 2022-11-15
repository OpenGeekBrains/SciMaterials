using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.ConsoleTests;

public class GetFileByIdTest
{
    private readonly IFilesClient _filesClient;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;

    public GetFileByIdTest(IFilesClient FilesClient, IUnitOfWork<SciMaterialsContext> UnitOfWork)
    {
        _filesClient = FilesClient;
        _unitOfWork = UnitOfWork;
    }

    public async Task Get(Guid fileId)
    {
        var result = await _filesClient.GetByIdAsync(fileId);

        if (result.Succeeded)
            Console.WriteLine($"{result.Data.Id} >>> {result.Data.Name}");
        else
            Console.WriteLine(result.Message);
    }
}
