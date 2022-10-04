using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SciMaterials.FilesApiTests;

public class TestSample : IAsyncLifetime
{
    private WebApplicationFactory<Program> _webHost = null!;
    private HttpClient _httpClient = null!;

    public Task InitializeAsync()
    {
        _webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(b => { });
        _httpClient = _webHost.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _webHost.DisposeAsync();
    }

    [Fact]
    public async Task CheckIsOk()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("api/TestSample");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}