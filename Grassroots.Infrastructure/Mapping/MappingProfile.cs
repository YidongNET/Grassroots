using AutoMapper;
using Grassroots.Model.Entities;
using Grassroots.Model.DTO;

namespace Grassroots.Infrastructure.Mapping
{
    /// <summary>
    /// AutoMapper映射配置
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MappingProfile()
        {
            // 实体到DTO的映射
            CreateMap<Todo, TodoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
            
            // DTO到实体的映射
            CreateMap<TodoDto, Todo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate));
        }
    }
} 