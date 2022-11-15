using System.Reflection;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.ConsoleTests;

public class DownloadFileByIdTest
{
    private readonly IFilesClient _filesClient;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;

    public DownloadFileByIdTest(IFilesClient filesClient, IUnitOfWork<SciMaterialsContext> unitOfWork)
    {
        _filesClient = filesClient;
        _unitOfWork = unitOfWork;
    }

    public async Task Download(Guid fileId)
    {
        // var result = await _filesClient.DownloadByIdAsync(fileId);
        var result = await _filesClient.DownloadByHashAsync("73B1C52BF527FC7A21E4FC3CCA36CC240DC047FF27F6D3275F4435D4E1938BF404CC406D851207D69D723E979D750E2A9599490C157AF7047CF606D3CA1FF78B");

        if (result.Succeeded)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), result.Data.FileName);
            File.Delete(path);
            using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                await result.Data.FileStream.CopyToAsync(fs);
            }
        }
        else
            Console.WriteLine(result.Message);
    }
}
