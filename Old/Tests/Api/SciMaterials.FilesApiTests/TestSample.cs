using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;

using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.FilesApiTests;

public class TestSample : IAsyncLifetime
{
    private WebApplicationFactory<Program> _Host = null!;

    public Task InitializeAsync()
    {
        _Host    = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(b => b
               .ConfigureLogging(opt => opt.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning))
               .ConfigureServices(services => services
                       .RemoveAll<SciMaterialsContext>()
                       .RemoveAll<DbContextOptions<SciMaterialsContext>>()
                       .AddDbContext<SciMaterialsContext>(opt => opt
                           .UseInMemoryDatabase("SciMaterialDB")
                           .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                           .EnableSensitiveDataLogging()
                        )
                       .RemoveAll<AuthDbContext>()
                       .RemoveAll<DbContextOptions<AuthDbContext>>()
                       .AddDbContext<AuthDbContext>(opt => opt
                           .UseInMemoryDatabase("SciMaterials.AuthDB")
                           .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                //.AddDbContext<SciMaterialsContext>(opt => opt.UseSqlite("Filename=:memory:"))
                ));

        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _Host.DisposeAsync();

    [Fact]
    public async Task CheckIsOk()
    {
        var response = await _Host.CreateClient().GetAsync("api/Test/check");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}