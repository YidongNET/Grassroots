using AutoMapper;
using Grassroots.Application.Dtos;
using Grassroots.Domain.Entities;

namespace Grassroots.Application.Mappings;

/// <summary>
/// AutoMapper映射配置文件
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 用户映射配置
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt));

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

        // 在这里添加映射配置
        // 例如：
        // CreateMap<SourceType, DestinationType>();
        // CreateMap<DestinationType, SourceType>();
        
        // 配置示例：
        // CreateMap<User, UserDto>()
        //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        //     .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.BirthDate.Year));
    }
} 