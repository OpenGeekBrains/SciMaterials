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
        [Inject] private ITagsClient TagsClient { get; set; }
        [Inject] private IFilesClient FilesClient { get; set; }
        [Inject] private IDialogService _DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        private IEnumerable<GetFileResponse>? _Files = new List<GetFileResponse>();
        private GetTagResponse? _Tag { get; set; }
        private string? _SearchString { get; set; }
        private bool _IsLoading = true;

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            
            var result = await TagsClient.GetByIdAsync(Id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _Tag = result.Data;
                
                var filesResult = await FilesClient.GetAllAsync().ConfigureAwait(false);
                _Files = filesResult.Data;

                _IsLoading = false;
            }
        }
        
        private void OnBackClick()
        {
            NavigationManager.NavigateTo("/tags_storage");
        }

        private async Task OnAddClickAsync()
        {
            var options = new DialogOptions()
            {
                CloseOnEscapeKey = true,
                MaxWidth         = MaxWidth.Medium, 
                FullWidth = true, 
                DisableBackdropClick = true
            };

            _DialogService.Show<TagsAddDialog>(
                title: "Add new tag",
                options: options);
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