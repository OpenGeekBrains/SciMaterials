using System.Collections.Immutable;

using Fluxor;

using SciMaterials.UI.BWASM.Models;
using SciMaterials.UI.BWASM.Services;
using SciMaterials.UI.BWASM.States.FilesUploadHistory;

namespace SciMaterials.UI.BWASM.States.UploadFilesForm.Behavior;

public class UploadFilesFormEffects
{
    private readonly FileUploadScheduleService _fileUploadScheduleService;

    public UploadFilesFormEffects(FileUploadScheduleService fileUploadScheduleService)
    {
        _fileUploadScheduleService = fileUploadScheduleService;
    }

    [EffectMethod]
    public async Task RegisterUploadData(UploadFilesFormActions.RegisterUploadDataAction action, IDispatcher dispatcher)
    {
        var uploadStates = Map(action).ToImmutableArray();
        dispatcher.Dispatch(UploadFilesFormActions.ClearForm());
        dispatcher.Dispatch(FilesUploadHistoryActions.RegisterFilesUpload(uploadStates));

        foreach (var file in Map(uploadStates))
        {
            await Task.Delay(100);
            _fileUploadScheduleService.ScheduleUpload(file);
        }
    }

    private static IEnumerable<FileUploadState> Map(UploadFilesFormActions.RegisterUploadDataAction data)
    {
        foreach (var dataFile in data.Files)
            yield return new(dataFile.BrowserFile, data.Category.Name, data.Category.Id)
            {
                AuthorId = data.Author.Id,
                AuthorName = data.Author.FirstName,
                Title = data.ShortInfo,
                Description = data.Description,
                CancellationSource = new()
            };
    }

    private static IEnumerable<FileUploadData> Map(IEnumerable<FileUploadState> data)
    {
        foreach (var dataFile in data)
            yield return new()
            {
                Id = dataFile.Id,
                File = dataFile.BrowserFile,
                FileName = dataFile.FileName,
                Category = dataFile.CategoryId,
                ShortInfo = dataFile.Title,
                AuthorId = dataFile.AuthorId,
                Description = dataFile.Description,
                CancellationToken = dataFile.CancellationSource.Token
            };
    }
}