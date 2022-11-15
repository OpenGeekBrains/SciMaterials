using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.WebApi.Clients.Files;

namespace SciMaterials.ConsoleTests;

public class EditFilesTest
{
    private readonly IFilesClient _FilesClient;

    public EditFilesTest(IFilesClient FilesClient, IUnitOfWork<SciMaterialsContext> UnitOfWork)
    {
        _FilesClient = FilesClient;
    }

    public async Task Edit(EditFileRequest request)
    {
        var result = await _FilesClient.EditAsync(request);
        if (result.Succeeded)
            Console.WriteLine($"Updated success >>> {result.Data}");
        else
            Console.WriteLine(result.Message);
    }
}
