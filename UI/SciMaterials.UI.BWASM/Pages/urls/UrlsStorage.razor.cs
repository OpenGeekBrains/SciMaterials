using System;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts.WebApi.Clients.Urls;

namespace SciMaterials.UI.BWASM.Pages.urls;

public partial class UrlsStorage
{
    [Inject] private IUrlsClient UrlsClient { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    private IEnumerable<GetUrlResponse> _data;
    private MudTable<GetUrlResponse> _table;

    private string? _searchString = null;

    private async Task<TableData<GetUrlResponse>> ServerReload(TableState state)
    {
        var result = await UrlsClient.GetAllAsync();
        if (result.Succeeded)
        {
            var data = result.Data;
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(_searchString))
                    return true;
                if (element.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            switch (state.SortLabel)
            {
                case "id_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "name_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Name);
                    break;
                case "title_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Title);
                    break;
                case "description_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Description);
                    break;
                case "link_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Link);
                    break;
            }

            _data = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<GetUrlResponse>() { TotalItems = result.Data.Count(), Items = _data };
        }

        return new TableData<GetUrlResponse>() { TotalItems = 0, Items = null };
    }

    private async Task AddAsync()
    {
        var parameters = new DialogParameters()
        {
            [nameof(AddEditUrlDialog.DialogType)] = DialogTypes.Add,
            [nameof(AddEditUrlDialog.AddEditUrlModel)] = new AddUrlRequest()
        };

        await ShowDialogAsync(parameters, "Create");
    }

    private async Task EditAsync(Guid? id)
    {
        var parameters = new DialogParameters()
        {
            [nameof(AddEditUrlDialog.DialogType)] = DialogTypes.Add,
        };
        if (id.HasValue)
        {
            var url = _data.FirstOrDefault();
            if (url is { })
            {
                parameters.Add(nameof(AddEditUrlDialog.EditEditUrlModel), new EditUrlRequest()
                {
                    Id = url.Id,
                    Name = url.Name,
                    Description = url.Description,
                    Title = url.Title,
                });
            }

        }
        await ShowDialogAsync(parameters, "Edit");
    }

    private async Task ShowDialogAsync(DialogParameters parameters, string title)
    {
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };

        var dialog = await DialogService.Show<AddEditUrlDialog>(
            title,
            parameters,
            options)
            .Result;

        if (!dialog.Cancelled)
        {
            OnSearch("");
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }
}
