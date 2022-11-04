using AutoMapper;

using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.API.Settings;

using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Contracts.API.Mappings;

public class FileProfile : Profile
{
    public FileProfile()
    {
        CreateMap<File, GetFileResponse>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom<FileTagResolver>())
            .ForMember(dest => dest.Categories, opt => opt.MapFrom<FileCategoryResolver>())
            .ReverseMap();
        // CreateMap<File, EditFileRequest>().ReverseMap();
        CreateMap<UploadFileRequest, FileMetadata>().ReverseMap();
        CreateMap<FileMetadata, File>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<EditFileRequest, File>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ReverseMap();
    }

    internal class FileTagResolver : IValueResolver<File, GetFileResponse, string>
    {
        private readonly string _separator;

        public FileTagResolver(IApiSettings apiSettings) => _separator = apiSettings.Separator;

        public string Resolve(File source, GetFileResponse destination, string destMember, ResolutionContext context)
            => string.Join(_separator, source.Tags.Select(t => t.Name));
    }


    internal class FileCategoryResolver : IValueResolver<File, GetFileResponse, string>
    {
        private readonly string _separator;

        public FileCategoryResolver(IApiSettings apiSettings) => _separator = apiSettings.Separator;

        public string Resolve(File source, GetFileResponse destination, string destMember, ResolutionContext context)
            => string.Join(_separator, source.Categories.Select(c => c.Id));
    }
}
