using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;


namespace SciMaterials.ConsoleTests;

public class GetAllFilesTest
{
    private readonly IFilesClient _filesClient;

    public GetAllFilesTest(IFilesClient FilesClient, IUnitOfWork<SciMaterialsContext> UnitOfWork)
    {
        _filesClient = FilesClient;
    }

    public async Task Get()
    {
        var result = await _filesClient.GetAllAsync();

        if (result.Succeeded)
            foreach (var item in result.Data)
                Console.WriteLine($"{item.Id} >>> {item.Name}");
        else
            Console.WriteLine(result.Message);
    }
}
