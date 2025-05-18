using Grassroots.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System;

namespace Grassroots.Infrastructure.IdGenerators;

/// <summary>
/// 雪花算法配置选项
/// </summary>
public class SnowflakeOptions
{
    /// <summary>
    /// 数据中心ID（0-31）
    /// </summary>
    public int DatacenterId { get; set; } = 0;

    /// <summary>
    /// 工作机器ID（0-31）
    /// </summary>
    public int WorkerId { get; set; } = 0;

    /// <summary>
    /// 开始时间戳 (2023-01-01 00:00:00 UTC)
    /// </summary>
    public long Epoch { get; set; } = 1672531200000L;

    /// <summary>
    /// 序列号位数 (默认12)
    /// </summary>
    public int SequenceBits { get; set; } = 12;

    /// <summary>
    /// 工作机器ID位数 (默认5)
    /// </summary>
    public int WorkerIdBits { get; set; } = 5;

    /// <summary>
    /// 数据中心ID位数 (默认5)
    /// </summary>
    public int DatacenterIdBits { get; set; } = 5;
}

/// <summary>
/// 雪花算法ID生成器实现
/// 分布式环境下用于生成唯一ID
/// 结构: 1位符号位 + 41位时间戳 + 5位数据中心ID + 5位工作机器ID + 12位序列号
/// </summary>
public class SnowflakeIdGenerator : IIdGenerator
{
    private readonly SnowflakeOptions _options;

    // 序列号和上次时间戳
    private long _sequence = 0L;
    private long _lastTimestamp = -1L;

    // 计算得到的值
    private readonly int _workerIdBits;
    private readonly int _datacenterIdBits;
    private readonly int _sequenceBits;
    private readonly int _maxWorkerId;
    private readonly int _maxDatacenterId;
    private readonly int _workerIdShift;
    private readonly int _datacenterIdShift;
    private readonly int _timestampLeftShift;
    private readonly int _sequenceMask;

    // 工作机器ID和数据中心ID
    private readonly int _workerId;
    private readonly int _datacenterId;

    // 对象锁，用于防止并发
    private readonly object _lock = new object();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">雪花算法配置选项</param>
    public SnowflakeIdGenerator(IOptions<SnowflakeOptions> options)
    {
        _options = options.Value;

        // 从配置中获取位数配置
        _workerIdBits = _options.WorkerIdBits;
        _datacenterIdBits = _options.DatacenterIdBits;
        _sequenceBits = _options.SequenceBits;

        // 计算最大值
        _maxWorkerId = -1 ^ (-1 << _workerIdBits);
        _maxDatacenterId = -1 ^ (-1 << _datacenterIdBits);

        // 计算位移
        _workerIdShift = _sequenceBits;
        _datacenterIdShift = _sequenceBits + _workerIdBits;
        _timestampLeftShift = _sequenceBits + _workerIdBits + _datacenterIdBits;

        // 计算序列掩码
        _sequenceMask = -1 ^ (-1 << _sequenceBits);

        // 支持WorkerId为-1的情况，自动生成一个随机值
        if (_options.WorkerId == -1)
        {
            _workerId = new Random().Next(0, _maxWorkerId + 1);
        }
        else
        {
            // 验证配置
            if (_options.WorkerId < 0 || _options.WorkerId > _maxWorkerId)
            {
                throw new ArgumentException($"Worker ID不能小于0或大于{_maxWorkerId}");
            }
            _workerId = _options.WorkerId;
        }

        // 验证数据中心ID
        if (_options.DatacenterId < 0 || _options.DatacenterId > _maxDatacenterId)
        {
            throw new ArgumentException($"Datacenter ID不能小于0或大于{_maxDatacenterId}");
        }
        _datacenterId = _options.DatacenterId;
    }

    /// <summary>
    /// 生成新的唯一ID
    /// </summary>
    /// <returns>唯一ID</returns>
    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = TimeGen();

            // 如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException(
                    $"时钟回退异常，拒绝生成ID，持续时间: {_lastTimestamp - timestamp} 毫秒");
            }

            // 如果是同一时间生成的，则进行序列号递增
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & _sequenceMask;
                // 序列号用尽，等待下一毫秒
                if (_sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            // 如果是新的毫秒，序列号重置为0
            else
            {
                _sequence = 0L;
            }

            // 记录最后一次生成ID的时间戳
            _lastTimestamp = timestamp;

            // 组合成64位ID
            return ((timestamp - _options.Epoch) << _timestampLeftShift) | // 时间戳部分
                   (_datacenterId << _datacenterIdShift) |                 // 数据中心部分
                   (_workerId << _workerIdShift) |                         // 工作机器ID部分
                   _sequence;                                              // 序列号部分
        }
    }

    /// <summary>
    /// 等待下一毫秒
    /// </summary>
    /// <param name="lastTimestamp">上一次生成ID的时间戳</param>
    /// <returns>新的时间戳</returns>
    private long TilNextMillis(long lastTimestamp)
    {
        var timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }
        return timestamp;
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns>时间戳（毫秒）</returns>
    private long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
} 