using Microsoft.AspNetCore.Http.Features;
using SciMaterials.API.Data;
using SciMaterials.API.Data.Interfaces;
using SciMaterials.API.Helpers;
using SciMaterials.API.Services;
using SciMaterials.API.Services.Interfaces;
using SciMaterials.API.Services.Stores;

var builder = WebApplication.CreateBuilder(args);

var maxFileSize = builder.Configuration.GetValue<long>("MaxFileSize");
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = maxFileSize;
});

var services = builder.Services;
services.AddScoped<IFileService<Guid>, FileService>();
services.AddScoped<IFileStore, FileSystemStore>();
services.AddSingleton<IFileRepository<Guid>, FileInfoMemoryRepository>();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadFilter>();
});
services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxFileSize;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
