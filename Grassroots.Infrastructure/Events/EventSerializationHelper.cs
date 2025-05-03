using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Grassroots.Domain.Events;

namespace Grassroots.Infrastructure.Events
{
    /// <summary>
    /// 事件序列化助手
    /// </summary>
    public static class EventSerializationHelper
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        static EventSerializationHelper()
        {
            Options.Converters.Add(new JsonStringEnumConverter());
        }

        /// <summary>
        /// 序列化事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <returns>JSON字符串</returns>
        public static string SerializeEvent(DomainEvent @event)
        {
            return JsonSerializer.Serialize(@event, @event.GetType(), Options);
        }

        /// <summary>
        /// 序列化事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <returns>JSON字符串</returns>
        public static string SerializeEvent(IntegrationEvent @event)
        {
            return JsonSerializer.Serialize(@event, @event.GetType(), Options);
        }

        /// <summary>
        /// 反序列化领域事件
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <param name="eventType">事件类型</param>
        /// <returns>领域事件</returns>
        public static DomainEvent DeserializeDomainEvent(string json, string eventType)
        {
            var type = FindEventType(eventType);
            if (type == null)
            {
                throw new InvalidOperationException($"Cannot find event type: {eventType}");
            }

            return (DomainEvent)JsonSerializer.Deserialize(json, type, Options);
        }

        /// <summary>
        /// 反序列化集成事件
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <param name="eventType">事件类型</param>
        /// <returns>集成事件</returns>
        public static IntegrationEvent DeserializeIntegrationEvent(string json, string eventType)
        {
            var type = FindEventType(eventType);
            if (type == null)
            {
                throw new InvalidOperationException($"Cannot find event type: {eventType}");
            }

            return (IntegrationEvent)JsonSerializer.Deserialize(json, type, Options);
        }

        private static Type FindEventType(string eventType)
        {
            // 首先尝试从完全限定名获取类型
            var type = Type.GetType(eventType);
            if (type != null)
            {
                return type;
            }

            // 尝试从当前领域程序集查找
            var domainAssembly = typeof(DomainEvent).Assembly;
            foreach (var t in domainAssembly.GetTypes())
            {
                if (t.Name == eventType || t.FullName == eventType)
                {
                    return t;
                }
            }

            return null;
        }
    }
} 