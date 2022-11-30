using AutoMapper;

using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, GetAuthorResponse>().ReverseMap();
        CreateMap<Author, AddAuthorRequest>().ReverseMap();
        CreateMap<Author, EditAuthorRequest>().ReverseMap();
    }
}
