using Grassroots.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace Grassroots.Infrastructure.Services
{
    /// <summary>
    /// 雪花算法配置
    /// </summary>
    public class SnowflakeOptions
    {
        /// <summary>
        /// 工作机器ID (0-31)
        /// </summary>
        public int WorkerId { get; set; } = 0;
        
        /// <summary>
        /// 数据中心ID (0-31)
        /// </summary>
        public int DatacenterId { get; set; } = 0;
        
        /// <summary>
        /// 开始时间戳 (2023-01-01 00:00:00 UTC)
        /// </summary>
        public long Epoch { get; set; } = 1672531200000;
        
        /// <summary>
        /// 序列号占用位数
        /// </summary>
        public int SequenceBits { get; set; } = 12;
        
        /// <summary>
        /// 工作机器ID占用位数
        /// </summary>
        public int WorkerIdBits { get; set; } = 5;
        
        /// <summary>
        /// 数据中心ID占用位数
        /// </summary>
        public int DatacenterIdBits { get; set; } = 5;
    }
    
    /// <summary>
    /// 雪花算法ID生成器实现
    /// </summary>
    public class SnowflakeIdGenerator : IIdGenerator
    {
        private readonly SnowflakeOptions _options;
        
        // 各部分占位
        private readonly int _sequenceBits;
        private readonly int _workerIdBits;
        private readonly int _datacenterIdBits;
        
        // 最大值
        private readonly int _maxWorkerId;
        private readonly int _maxDatacenterId;
        private readonly int _maxSequence;
        
        // 偏移量
        private readonly int _workerIdShift;
        private readonly int _datacenterIdShift;
        private readonly int _timestampLeftShift;
        
        private long _sequence = 0L;                 // 序列号
        private long _lastTimestamp = -1L;           // 上次生成ID的时间戳
        
        private readonly object _lock = new object();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">配置选项</param>
        public SnowflakeIdGenerator(IOptions<SnowflakeOptions> options)
        {
            _options = options.Value;
            
            // 从配置读取位数参数
            _sequenceBits = _options.SequenceBits;
            _workerIdBits = _options.WorkerIdBits;
            _datacenterIdBits = _options.DatacenterIdBits;
            
            // 计算最大值
            _maxWorkerId = -1 ^ (-1 << _workerIdBits);
            _maxDatacenterId = -1 ^ (-1 << _datacenterIdBits);
            _maxSequence = -1 ^ (-1 << _sequenceBits);
            
            // 计算偏移量
            _workerIdShift = _sequenceBits;
            _datacenterIdShift = _sequenceBits + _workerIdBits;
            _timestampLeftShift = _sequenceBits + _workerIdBits + _datacenterIdBits;
            
            // 对于WorkerId为-1的特殊情况，使用默认值0
            if (_options.WorkerId < 0)
            {
                _options.WorkerId = 0;
                Console.WriteLine("Warning: WorkerId is set to a negative value. Using default value 0 instead.");
            }
            
            // 验证参数
            if (_options.WorkerId > _maxWorkerId)
            {
                throw new ArgumentException($"Worker ID must be between 0 and {_maxWorkerId}");
            }
            
            if (_options.DatacenterId < 0 || _options.DatacenterId > _maxDatacenterId)
            {
                throw new ArgumentException($"Datacenter ID must be between 0 and {_maxDatacenterId}");
            }
        }
        
        /// <summary>
        /// 生成新的ID
        /// </summary>
        /// <returns>唯一ID</returns>
        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = GetTimestamp();
                
                // 如果当前时间小于上次生成ID的时间，说明系统时钟回退，抛出异常
                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidOperationException(
                        $"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds");
                }
                
                // 如果是同一时间生成的，则进行序列号自增
                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _maxSequence;
                    // 序列号已达最大值，需要等待下一毫秒
                    if (_sequence == 0)
                    {
                        timestamp = WaitNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    // 时间戳改变，序列号重置
                    _sequence = 0L;
                }
                
                _lastTimestamp = timestamp;
                
                // 生成ID (64位)
                return ((timestamp - _options.Epoch) << _timestampLeftShift) | 
                       (_options.DatacenterId << _datacenterIdShift) |
                       (_options.WorkerId << _workerIdShift) | 
                       _sequence;
            }
        }
        
        /// <summary>
        /// 解析雪花ID
        /// </summary>
        /// <param name="id">雪花ID</param>
        /// <returns>解析结果</returns>
        public IdMetadata Analyze(long id)
        {
            var timestamp = (id >> _timestampLeftShift) + _options.Epoch;
            var dataCenterId = (id >> _datacenterIdShift) & ((1 << _datacenterIdBits) - 1);
            var workerId = (id >> _workerIdShift) & ((1 << _workerIdBits) - 1);
            var sequence = id & ((1 << _sequenceBits) - 1);
            
            return new IdMetadata
            {
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime,
                DatacenterId = (int)dataCenterId,
                WorkerId = (int)workerId,
                Sequence = (int)sequence
            };
        }
        
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns>毫秒时间戳</returns>
        private long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        /// <summary>
        /// 等待到下一毫秒
        /// </summary>
        /// <param name="lastTimestamp">上次生成ID的时间戳</param>
        /// <returns>新的时间戳</returns>
        private long WaitNextMillis(long lastTimestamp)
        {
            var timestamp = GetTimestamp();
            while (timestamp <= lastTimestamp)
            {
                Thread.Sleep(0); // 让出CPU
                timestamp = GetTimestamp();
            }
            return timestamp;
        }
    }
} 