using AutoMapper;
using Grassroots.Domain.Entities;
using Grassroots.Model.DTO;

namespace Grassroots.Infrastructure.Mapping
{
    /// <summary>
    /// AutoMapper映射配置
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 构造函数，配置映射
        /// </summary>
        public MappingProfile()
        {
            // 实体 -> DTO
            CreateMap<Todo, TodoDto>();
            
            // DTO -> 实体 (不使用，因为实体创建需要通过构造函数)
        }
    }
} 