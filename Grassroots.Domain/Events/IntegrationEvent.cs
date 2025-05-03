using System;
using System.Text.Json;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 集成事件基类
    /// </summary>
    public abstract class IntegrationEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime CreationDate { get; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType => GetType().Name;

        /// <summary>
        /// 事件元数据
        /// </summary>
        public JsonDocument Metadata { get; private set; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            Metadata = JsonDocument.Parse("{}");
        }

        /// <summary>
        /// 添加元数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void AddMetadata(string key, object value)
        {
            var metadataObject = new
            {
                existingMetadata = JsonSerializer.Deserialize<object>(Metadata.RootElement.GetRawText()),
                newData = new Dictionary<string, object> { { key, value } }
            };
            
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            var json = JsonSerializer.Serialize(metadataObject, jsonOptions);
            Metadata = JsonDocument.Parse(json);
        }

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public T GetMetadata<T>(string key)
        {
            if (Metadata.RootElement.TryGetProperty(key, out var element))
            {
                return JsonSerializer.Deserialize<T>(element.GetRawText());
            }
            
            return default;
        }
    }
} 