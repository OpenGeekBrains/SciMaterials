using AutoMapper;

using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class UrlProfile : Profile
{
    public UrlProfile()
    {
        CreateMap<Url, GetUrlResponse>().ReverseMap();
        CreateMap<Url, AddUrlRequest>().ReverseMap();
        CreateMap<Url, EditUrlRequest>().ReverseMap();
    }
}
