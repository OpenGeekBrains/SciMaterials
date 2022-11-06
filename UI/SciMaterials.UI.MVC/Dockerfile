#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UI/SciMaterials.UI.MVC/SciMaterials.UI.MVC.csproj", "UI/SciMaterials.UI.MVC/"]
COPY ["Data/Migrations/SciMaterials.Postgres.Auth.Migrations/SciMaterials.Postgres.Auth.Migrations.csproj", "Data/Migrations/SciMaterials.Postgres.Auth.Migrations/"]
COPY ["Data/SciMaterials.DAL.AUTH/SciMaterials.DAL.AUTH.csproj", "Data/SciMaterials.DAL.AUTH/"]
COPY ["Services/SciMaterials.Contracts/SciMaterials.Contracts.csproj", "Services/SciMaterials.Contracts/"]
COPY ["Data/Migrations/SciMaterials.SqlLite.Auth.Migrations/SciMaterials.SqlLite.Auth.Migrations.csproj", "Data/Migrations/SciMaterials.SqlLite.Auth.Migrations/"]
COPY ["Services/SciMaterials.Contracts.Identity.Clients/SciMaterials.Contracts.Identity.Clients.csproj", "Services/SciMaterials.Contracts.Identity.Clients/"]
COPY ["Services/SciMaterials.Contracts.Identity.API/SciMaterials.Contracts.Identity.API.csproj", "Services/SciMaterials.Contracts.Identity.API/"]
COPY ["Services/SciMaterials.Contracts.API/SciMaterials.Contracts.API.csproj", "Services/SciMaterials.Contracts.API/"]
COPY ["Data/SciMaterials.DAL/SciMaterials.DAL.csproj", "Data/SciMaterials.DAL/"]
COPY ["Services/SciMaterials.Services.API/SciMaterials.Services.API.csproj", "Services/SciMaterials.Services.API/"]
COPY ["Common/SciMaterials.RepositoryLib/SciMaterials.RepositoryLib.csproj", "Common/SciMaterials.RepositoryLib/"]
COPY ["Services/SciMaterials.WebApi.Clients/SciMaterials.WebApi.Clients.csproj", "Services/SciMaterials.WebApi.Clients/"]
COPY ["Services/SciMaterials.Contracts.WebApi.Clients/SciMaterials.Contracts.WebApi.Clients.csproj", "Services/SciMaterials.Contracts.WebApi.Clients/"]
COPY ["Services/SciMaterials.Services.Database/SciMaterials.Services.Database.csproj", "Services/SciMaterials.Services.Database/"]
COPY ["Services/SciMaterials.Contracts.Database/SciMaterials.Contracts.Database.csproj", "Services/SciMaterials.Contracts.Database/"]
COPY ["Data/Migrations/SciMaterials.Data.MySqlMigrations/SciMaterials.Data.MySqlMigrations.csproj", "Data/Migrations/SciMaterials.Data.MySqlMigrations/"]
COPY ["Data/Migrations/SciMaterials.MsSqlServerMigrations/SciMaterials.MsSqlServerMigrations.csproj", "Data/Migrations/SciMaterials.MsSqlServerMigrations/"]
COPY ["Data/Migrations/SciMaterials.PostgresqlMigrations/SciMaterials.PostgresqlMigrations.csproj", "Data/Migrations/SciMaterials.PostgresqlMigrations/"]
COPY ["Data/Migrations/SciMaterials.SQLiteMigrations/SciMaterials.SQLiteMigrations.csproj", "Data/Migrations/SciMaterials.SQLiteMigrations/"]
COPY ["UI/SciMaterials.UI.BWASM/SciMaterials.UI.BWASM.csproj", "UI/SciMaterials.UI.BWASM/"]
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