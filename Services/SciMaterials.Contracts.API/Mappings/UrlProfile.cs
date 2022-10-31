using AutoMapper;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class UrlProfile : Profile
{
    public UrlProfile()
    {
        CreateMap<Url, GetUrlResponse>().ReverseMap();
        CreateMap<Url, AddUrlRequest>().ReverseMap();
        CreateMap<Url, EditUrlRequest>().ReverseMap();
    }
}
