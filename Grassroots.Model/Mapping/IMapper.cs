namespace Grassroots.Model.Mapping
{
    /// <summary>
    /// 对象映射接口
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>目标对象</returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// 映射到现有对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns>目标对象</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
} 