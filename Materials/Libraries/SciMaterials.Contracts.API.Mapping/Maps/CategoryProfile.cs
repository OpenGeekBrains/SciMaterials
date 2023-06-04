using AutoMapper;

using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, GetCategoryResponse>().ReverseMap();
        CreateMap<Category, AddCategoryRequest>().ReverseMap();
        CreateMap<Category, EditCategoryRequest>().ReverseMap();
    }
}
