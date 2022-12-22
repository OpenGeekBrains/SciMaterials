using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Files;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsView
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ITagsClient _TagsClient { get; set; }
        [Inject] private IFilesClient _FilesClient { get; set; }
        [Inject] private NavigationManager _NavigationManager { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }

        private IEnumerable<GetFileResponse>? _Files = new List<GetFileResponse>();
        private GetTagResponse? _Tag { get; set; }
        private string? _SearchString { get; set; }
        private bool _IsLoading = true;

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            var result = await _TagsClient.GetByIdAsync(Id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _Tag = result.Data;
                
                var filesResult = await _FilesClient.GetAllAsync().ConfigureAwait(false);
                _Files     = filesResult.Data;
                _IsLoading = false;
            }
        }
        
        private void OnBackClick()
        {
            _NavigationManager.NavigateTo("/tags_storage");
        }
        
        private async Task OnLinkClickAsync(Guid? FileId)
        {
            _NavigationManager.NavigateTo($"/filedetails/{FileId}");
        }
        
        private async Task OnEditClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey     = true,
                MaxWidth             = MaxWidth.Medium, 
                FullWidth            = true, 
                DisableBackdropClick = true
            };

            var parameters = new DialogParameters();
            parameters.Add(nameof(TagsEditDialog.EditTagsModel), new EditTagRequest() {Id = Id});
            
            await _DialogService.ShowAsync<TagsEditDialog>(
                title: "Edit tag name",
                options: options,
                parameters: parameters).ConfigureAwait(false);
        }
        
        private async Task OnDeleteClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey     = true,
                MaxWidth             = MaxWidth.Medium, 
                FullWidth            = true, 
                DisableBackdropClick = true
            };

            var parameters = new DialogParameters();
            parameters.Add(nameof(TagsDeleteDialog.DeleteTagsModel), new EditTagRequest() {Id = Id});
            
            await _DialogService.ShowAsync<TagsDeleteDialog>(
                title: "Delete tag",
                options: options,
                parameters: parameters).ConfigureAwait(false);
        }
        
        private Func<GetFileResponse, bool> _QuickSearch => (x) =>
        {
            if (string.IsNullOrWhiteSpace(_SearchString))
                return true;
            
            if (x.Name.ToString().Contains(_SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            if (x.Description.ToString().Contains(_SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        };
    }
}