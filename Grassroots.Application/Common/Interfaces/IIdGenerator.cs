using System;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// ID生成器接口
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// 生成新的ID
        /// </summary>
        /// <returns>唯一ID</returns>
        long NextId();
        
        /// <summary>
        /// 解析雪花ID
        /// </summary>
        /// <param name="id">雪花ID</param>
        /// <returns>解析结果</returns>
        IdMetadata Analyze(long id);
    }
    
    /// <summary>
    /// ID元数据
    /// </summary>
    public class IdMetadata
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 工作机器ID
        /// </summary>
        public int WorkerId { get; set; }
        
        /// <summary>
        /// 数据中心ID
        /// </summary>
        public int DatacenterId { get; set; }
        
        /// <summary>
        /// 序列号
        /// </summary>
        public int Sequence { get; set; }
    }
} 