using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SciMaterials.DAL.Contexts;

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices);


static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddDbContext<SciMaterialsContext>(options => options.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=SciMaterials;Integrated Security=True"));
}

using IHost host = CreateHostBuilder(args).Build();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();