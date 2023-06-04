using AutoMapper;
using Microsoft.Extensions.Logging;

using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.UnitOfWork;


namespace SciMaterials.Services.API;

public abstract class ApiServiceBase: ServiceBase
{
    protected readonly IUnitOfWork<SciMaterialsContext> Database;
    protected readonly IMapper _Mapper;

    protected ApiServiceBase(IUnitOfWork<SciMaterialsContext> Database, IMapper Mapper, ILogger<ApiServiceBase> Logger) 
        : base(Logger)
    {
        this.Database = Database;
        _Mapper = Mapper;
    }
}