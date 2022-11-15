using System.Net.Http.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Identity.API.DTO.Users;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;
using SciMaterials.DAL.AUTH.Context;
using SciMaterials.DAL.Contexts;

namespace SciMaterials.UI.MVC.Tests.Controllers.API.Identity;

public class AccountControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _Host = null!;


    public Task InitializeAsync()
    {
        //var file_store_mock = new Mock<IFileStore>();
        //file_store_mock.Setup(s => s.WriteAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
        //   .ReturnsAsync(
        //        (FileName, DataStream, Cancel) =>
        //        {
        //            return new FileWriteResult("Hash", 123);
        //        });

        _Host = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(builder => builder
               .ConfigureLogging(opt => opt.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning))
               .ConfigureServices(services => services
                   .RemoveAll<SciMaterialsContext>()
                   .RemoveAll<DbContextOptions<SciMaterialsContext>>()
                   .AddDbContext<SciMaterialsContext>(opt => opt
                       .UseInMemoryDatabase("SciMaterialDB")
                       .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    )
                   .RemoveAll<AuthDbContext>()
                   .RemoveAll<DbContextOptions<AuthDbContext>>()
                   .AddDbContext<AuthDbContext>(opt => opt
                       .UseInMemoryDatabase("SciMaterials.AuthDB")
                       .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                //.AddDbContext<SciMaterialsContext>(opt => opt.UseSqlite("Filename=:memory:"))
                //.RemoveAll<IFileService>()
                //.AddScoped(_ => file_store_mock.Object)
                ));

        //WebApplicationFactory<Startup> webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        //{
        //    builder.ConfigureTestServices(services =>
        //    {
        //        var dbContextDescriptor = services.SingleOrDefault(d =>
        //            d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        //        services.Remove(dbContextDescriptor);

        //        services.AddDbContext<ApplicationDbContext>(options =>
        //        {
        //            options.UseInMemoryDatabase("delivery_db");
        //        });
        //    });
        //});

        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _Host.DisposeAsync();

    [Fact]
    public async Task RegisterAsync_RegisterNewUser_Success()
    {
        //var db = _Host.Services.GetRequiredService<SciMaterialsContext>();

        var http = _Host.CreateClient();

        var auth_address = $"{AuthApiRoute.AuthControllerName}/{AuthApiRoute.Register}";

        var response = await http.PostAsJsonAsync(auth_address, new RegisterRequest
        {
            Email = "test@server.ru",
            NickName = "qwe",
            Password = "123321_qweEWQ"
        });

        var result = response.EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<ClientCreateUserResponse>();

        Assert.NotNull(result.Result);
        Assert.Equal(0, result.Result.Code);
        Assert.True(result.Result.Succeeded);
    }
}