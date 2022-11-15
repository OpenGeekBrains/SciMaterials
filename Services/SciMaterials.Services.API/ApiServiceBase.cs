using AutoMapper;
using Microsoft.Extensions.Logging;
using SciMaterials.DAL.Contexts;
using SciMaterials.DAL.UnitOfWork;


namespace SciMaterials.Services.API;

public abstract class ApiServiceBase: ServiceBase
{
    protected readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    protected readonly IMapper _mapper;

    protected ApiServiceBase(IUnitOfWork<SciMaterialsContext> unitOfWork, IMapper mapper, ILogger logger) 
        : base(logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
}