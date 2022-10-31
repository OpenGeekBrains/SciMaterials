using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.Models;
using SciMaterials.DAL.UnitOfWork;

namespace SciMaterials.ConsoleTests;

public class SendFileTest
{
    private readonly IFilesClient _FilesClient;
    private readonly IUnitOfWork<SciMaterialsContext> _UnitOfWork;

    public SendFileTest(IFilesClient FilesClient, IUnitOfWork<SciMaterialsContext> UnitOfWork)
    {
        _FilesClient = FilesClient;
        _UnitOfWork = UnitOfWork;
    }

    public async Task SendFileAsync(string path)
    {
        var all_categories = await _UnitOfWork.GetRepository<Category>().GetAllAsync();
        var category_id = all_categories.First().Id;

        var all_authors = await _UnitOfWork.GetRepository<Author>().GetAllAsync();
        var author_id = all_authors.First().Id;

        var file_info = new FileInfo(path);
        using var file_stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        // var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name) + ".ee";
        var file_name = file_info.Name;
        var upload_file_request = new UploadFileRequest
        {
            Name = file_name,
            ShortInfo = "Файл " + file_info.Name,
            Description = "Содержит файл " + file_info.Name,
            Size = file_info.Length,
            Tags = "текст,книга,txt",
            Categories = category_id.ToString(),
            ContentTypeName = "text/plain",
            AuthorId = author_id
        };

        var result = await _FilesClient.UploadAsync(file_stream, upload_file_request);
        Console.WriteLine(string.Join(";", result.Messages));
    }
}
