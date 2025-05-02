using AutoMapper;
using Grassroots.Model.Mapping;
using IMapperInterface = Grassroots.Model.Mapping.IMapper;

namespace Grassroots.Infrastructure.Mapping
{
    /// <summary>
    /// AutoMapper适配器
    /// </summary>
    public class AutoMapperAdapter : Model.Mapping.IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapper">AutoMapper实例</param>
        public AutoMapperAdapter(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标对象</returns>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 映射到现有对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns>目标对象</returns>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }
    }
} 