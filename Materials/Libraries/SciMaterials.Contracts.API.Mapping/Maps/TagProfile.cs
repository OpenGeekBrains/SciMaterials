using AutoMapper;

using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Tag, GetTagResponse>().ReverseMap();
        CreateMap<Tag, AddTagRequest>().ReverseMap();
        CreateMap<Tag, EditTagRequest>().ReverseMap();
    }
}
