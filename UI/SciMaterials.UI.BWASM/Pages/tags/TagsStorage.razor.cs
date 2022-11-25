using Microsoft.AspNetCore.Components;
using MudBlazor;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.UI.BWASM.Pages.tags
{
    public partial class TagsStorage
    {
        [Inject] private ITagsClient TagsClient { get; set; }
        
        private RenderFragment _panelContent;

        private async Task ExpandedChanged(bool newVal)
        {
            if (newVal)
            {
                _panelContent = _bigAsyncContent;
            }
            else
            {
                Task.Delay(350).ContinueWith(t => _panelContent = null).AndForget(); 
            }
        }
    }
}