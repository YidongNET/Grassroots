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
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

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