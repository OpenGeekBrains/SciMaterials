using AutoMapper;

using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.DAL.Resources.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mapping.Maps;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, GetCommentResponse>().ReverseMap();
        CreateMap<Comment, AddCommentRequest>().ReverseMap();
        CreateMap<Comment, EditCommentRequest>().ReverseMap();
    }
}
