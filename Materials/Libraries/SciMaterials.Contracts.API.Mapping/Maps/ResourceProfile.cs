using AutoMapper;
using SciMaterials.Contracts.API.DTO.Resources;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class ResourceProfile : Profile
{
    public ResourceProfile()
    {
        CreateMap<Resource, GetResourceResponse>()
            .ReverseMap();
    }
}
