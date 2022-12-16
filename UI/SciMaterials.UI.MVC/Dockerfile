FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["UI/SciMaterials.UI.MVC/SciMaterials.UI.MVC.csproj", "UI/SciMaterials.UI.MVC/"]
COPY ["Data/Migrations/SciMaterials.Data.MySqlMigrations/SciMaterials.Data.MySqlMigrations.csproj", "Data/Migrations/SciMaterials.Data.MySqlMigrations/"]
COPY ["Data/SciMaterials.DAL.Resources/SciMaterials.DAL.Resources.csproj", "Data/SciMaterials.DAL.Resources/"]
COPY ["Data/SciMaterials.DAL.Contracts/SciMaterials.DAL.Contracts.csproj", "Data/SciMaterials.DAL.Contracts/"]
COPY ["Data/SciMaterials.DAL.Resources.Contracts/SciMaterials.DAL.Resources.Contracts.csproj", "Data/SciMaterials.DAL.Resources.Contracts/"]
COPY ["Data/Migrations/SciMaterials.MsSqlServerMigrations/SciMaterials.MsSqlServerMigrations.csproj", "Data/Migrations/SciMaterials.MsSqlServerMigrations/"]
COPY ["Data/Migrations/SciMaterials.MySql.Auth.Migrations/SciMaterials.MySql.Auth.Migrations.csproj", "Data/Migrations/SciMaterials.MySql.Auth.Migrations/"]
COPY ["Data/SciMaterials.DAL.AUTH/SciMaterials.DAL.AUTH.csproj", "Data/SciMaterials.DAL.AUTH/"]
COPY ["Services/SciMaterials.Contracts/SciMaterials.Contracts.csproj", "Services/SciMaterials.Contracts/"]
COPY ["Data/SciMaterials.DAL.AUTH.Contracts/SciMaterials.DAL.AUTH.Contracts.csproj", "Data/SciMaterials.DAL.AUTH.Contracts/"]
COPY ["Data/Migrations/SciMaterials.Postgres.Auth.Migrations/SciMaterials.Postgres.Auth.Migrations.csproj", "Data/Migrations/SciMaterials.Postgres.Auth.Migrations/"]
COPY ["Data/Migrations/SciMaterials.PostgresqlMigrations/SciMaterials.PostgresqlMigrations.csproj", "Data/Migrations/SciMaterials.PostgresqlMigrations/"]
COPY ["Data/Migrations/SciMaterials.SQLiteMigrations/SciMaterials.SQLiteMigrations.csproj", "Data/Migrations/SciMaterials.SQLiteMigrations/"]
COPY ["Data/Migrations/SciMaterials.SqlLite.Auth.Migrations/SciMaterials.SqlLite.Auth.Migrations.csproj", "Data/Migrations/SciMaterials.SqlLite.Auth.Migrations/"]
COPY ["Services/SciMaterials.Services.API/SciMaterials.Services.API.csproj", "Services/SciMaterials.Services.API/"]
COPY ["Services/SciMaterials.Contracts.API.Mapping/SciMaterials.Contracts.API.Mapping.csproj", "Services/SciMaterials.Contracts.API.Mapping/"]
COPY ["Services/SciMaterials.Contracts.API/SciMaterials.Contracts.API.csproj", "Services/SciMaterials.Contracts.API/"]
COPY ["Services/SciMaterials.Services.ShortLinks/SciMaterials.Services.ShortLinks.csproj", "Services/SciMaterials.Services.ShortLinks/"]
COPY ["Services/SciMaterials.Contracts.ShortLinks/SciMaterials.Contracts.ShortLinks.csproj", "Services/SciMaterials.Contracts.ShortLinks/"]
COPY ["Services/SciMaterials.Services/SciMaterials.Services.csproj", "Services/SciMaterials.Services/"]
COPY ["Services/SciMaterials.WebApi.Clients/SciMaterials.WebApi.Clients.csproj", "Services/SciMaterials.WebApi.Clients/"]
COPY ["Services/SciMaterials.Contracts.WebApi.Clients/SciMaterials.Contracts.WebApi.Clients.csproj", "Services/SciMaterials.Contracts.WebApi.Clients/"]
COPY ["UI/SciMaterials.UI.BWASM/SciMaterials.UI.BWASM.csproj", "UI/SciMaterials.UI.BWASM/"]
COPY ["Services/SciMaterials.Services.Identity.API/SciMaterials.Services.Identity.API.csproj", "Services/SciMaterials.Services.Identity.API/"]
COPY ["Services/SciMaterials.Contracts.Identity.API/SciMaterials.Contracts.Identity.API.csproj", "Services/SciMaterials.Contracts.Identity.API/"]

RUN dotnet restore "UI/SciMaterials.UI.MVC/SciMaterials.UI.MVC.csproj"

COPY . .

WORKDIR "/src/UI/SciMaterials.UI.MVC"
RUN dotnet build "SciMaterials.UI.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SciMaterials.UI.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SciMaterials.UI.MVC.dll"]