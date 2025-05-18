using System.Text.Json;
using Grassroots.Domain.Events;
using Grassroots.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.Events;

/// <summary>
/// 事件记录实体
/// </summary>
public class EventRecord
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 聚合根ID（字符串形式）
    /// </summary>
    public string AggregateId { get; set; }
    
    /// <summary>
    /// 事件类型名称
    /// </summary>
    public string EventType { get; set; }
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; set; }
    
    /// <summary>
    /// 事件数据（JSON格式）
    /// </summary>
    public string EventData { get; set; }
    
    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }
}

/// <summary>
/// 事件存储实现
/// </summary>
public class EventStore : IEventStore
{
    private readonly GrassrootsDbContext _dbContext;
    private readonly IDomainEventService _domainEventService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="domainEventService">领域事件服务</param>
    public EventStore(GrassrootsDbContext dbContext, IDomainEventService domainEventService)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _domainEventService = domainEventService ?? throw new ArgumentNullException(nameof(domainEventService));
        
        // 确保EventRecords表存在
        _dbContext.Database.EnsureCreated();
    }
    
    /// <summary>
    /// 保存事件
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="events">领域事件集合</param>
    /// <param name="expectedVersion">预期版本号</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> SaveEventsAsync<TKey>(TKey aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
    {
        // 获取当前聚合根的最新版本
        var aggregateIdStr = aggregateId.ToString();
        var latestVersion = await _dbContext.Set<EventRecord>()
            .Where(e => e.AggregateId == aggregateIdStr)
            .MaxAsync(e => (int?)e.Version) ?? 0;
        
        // 检查版本一致性
        if (expectedVersion != 0 && latestVersion != expectedVersion)
        {
            throw new DbUpdateConcurrencyException($"Concurrency conflict. Expected version {expectedVersion}, but got {latestVersion}");
        }
        
        var version = expectedVersion;
        
        // 保存新事件
        foreach (var @event in events)
        {
            version++;
            
            var eventRecord = new EventRecord
            {
                Id = @event.EventId,
                AggregateId = aggregateIdStr,
                EventType = @event.EventType,
                OccurredOn = @event.OccurredOn,
                EventData = JsonSerializer.Serialize(@event),
                Version = version
            };
            
            await _dbContext.Set<EventRecord>().AddAsync(eventRecord);
        }
        
        await _dbContext.SaveChangesAsync();
        
        // 发布领域事件
        await _domainEventService.PublishAsync(events);
        
        return true;
    }
    
    /// <summary>
    /// 获取聚合根的事件历史
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <returns>事件历史列表</returns>
    public async Task<IEnumerable<IDomainEvent>> GetEventsForAggregateAsync<TKey>(TKey aggregateId)
    {
        var aggregateIdStr = aggregateId.ToString();
        var eventRecords = await _dbContext.Set<EventRecord>()
            .Where(e => e.AggregateId == aggregateIdStr)
            .OrderBy(e => e.Version)
            .ToListAsync();
        
        return DeserializeEvents(eventRecords);
    }
    
    /// <summary>
    /// 获取指定类型的所有事件
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <returns>事件列表</returns>
    public async Task<IEnumerable<IDomainEvent>> GetEventsOfTypeAsync<T>() where T : IDomainEvent
    {
        var eventTypeName = typeof(T).Name;
        var eventRecords = await _dbContext.Set<EventRecord>()
            .Where(e => e.EventType == eventTypeName)
            .OrderBy(e => e.OccurredOn)
            .ToListAsync();
        
        return DeserializeEvents(eventRecords);
    }
    
    /// <summary>
    /// 反序列化事件记录为领域事件
    /// </summary>
    /// <param name="eventRecords">事件记录列表</param>
    /// <returns>领域事件列表</returns>
    private IEnumerable<IDomainEvent> DeserializeEvents(IEnumerable<EventRecord> eventRecords)
    {
        var events = new List<IDomainEvent>();
        
        foreach (var record in eventRecords)
        {
            // 获取事件类型
            var eventType = Type.GetType(record.EventType);
            if (eventType == null)
            {
                // 尝试从已加载的程序集中查找
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    eventType = assembly.GetTypes()
                        .FirstOrDefault(t => t.Name == record.EventType && typeof(IDomainEvent).IsAssignableFrom(t));
                    
                    if (eventType != null)
                        break;
                }
                
                if (eventType == null)
                    throw new Exception($"Event type {record.EventType} not found");
            }
            
            // 反序列化事件
            var @event = (IDomainEvent)JsonSerializer.Deserialize(record.EventData, eventType);
            events.Add(@event);
        }
        
        return events;
    }
} 