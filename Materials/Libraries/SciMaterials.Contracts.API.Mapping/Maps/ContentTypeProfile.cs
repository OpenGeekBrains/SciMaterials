using AutoMapper;

using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class ContentTypeProfile : Profile
{
    public ContentTypeProfile()
    {
        CreateMap<ContentType, GetContentTypeResponse>().ReverseMap();
        CreateMap<ContentType, AddContentTypeRequest>().ReverseMap();
        CreateMap<ContentType, EditContentTypeRequest>().ReverseMap();
    }
}
