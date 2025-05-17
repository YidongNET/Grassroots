using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grassroots.Api.Converters
{
    /// <summary>
    /// 长整型数值转字符串JSON转换器
    /// 用于解决JavaScript中大整数精度问题
    /// 将Int64, UInt64和Decimal类型在JSON序列化时自动转为字符串
    /// </summary>
    public class LongToStringConverter : JsonConverterFactory
    {
        /// <summary>
        /// 确定转换器是否可以转换指定的类型
        /// </summary>
        /// <param name="typeToConvert">要转换的类型</param>
        /// <returns>如果是long、ulong或decimal类型，返回true</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(long) ||
                   typeToConvert == typeof(ulong) ||
                   typeToConvert == typeof(long?) ||
                   typeToConvert == typeof(ulong?) ||
                   typeToConvert == typeof(decimal) ||
                   typeToConvert == typeof(decimal?);
        }

        /// <summary>
        /// 创建特定类型的JSON转换器
        /// </summary>
        /// <param name="typeToConvert">要转换的类型</param>
        /// <param name="options">序列化选项</param>
        /// <returns>类型转换器</returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(long))
                return new LongConverter();
            if (typeToConvert == typeof(ulong))
                return new ULongConverter();
            if (typeToConvert == typeof(long?))
                return new NullableLongConverter();
            if (typeToConvert == typeof(ulong?))
                return new NullableULongConverter();
            if (typeToConvert == typeof(decimal))
                return new DecimalConverter();
            if (typeToConvert == typeof(decimal?))
                return new NullableDecimalConverter();

            throw new ArgumentException($"无法转换类型 {typeToConvert}");
        }

        /// <summary>
        /// Decimal类型转换器
        /// </summary>
        private class DecimalConverter : JsonConverter<decimal>
        {
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (decimal.TryParse(stringValue, out decimal value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 decimal");
                }
                return reader.GetDecimal();
            }

            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        /// <summary>
        /// 可空Decimal类型转换器
        /// </summary>
        private class NullableDecimalConverter : JsonConverter<decimal?>
        {
            public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return null;
                    }
                    if (decimal.TryParse(stringValue, out decimal value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 decimal?");
                }
                return reader.GetDecimal();
            }

            public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                {
                    writer.WriteStringValue(value.Value.ToString());
                }
                else
                {
                    writer.WriteNullValue();
                }
            }
        }

        /// <summary>
        /// Int64(long)类型转换器
        /// </summary>
        private class LongConverter : JsonConverter<long>
        {
            public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (long.TryParse(stringValue, out long value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 long");
                }
                return reader.GetInt64();
            }

            public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        /// <summary>
        /// UInt64(ulong)类型转换器
        /// </summary>
        private class ULongConverter : JsonConverter<ulong>
        {
            public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (ulong.TryParse(stringValue, out ulong value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 ulong");
                }
                return reader.GetUInt64();
            }

            public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        /// <summary>
        /// 可空Int64(long?)类型转换器
        /// </summary>
        private class NullableLongConverter : JsonConverter<long?>
        {
            public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return null;
                    }
                    if (long.TryParse(stringValue, out long value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 long?");
                }
                return reader.GetInt64();
            }

            public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                {
                    writer.WriteStringValue(value.Value.ToString());
                }
                else
                {
                    writer.WriteNullValue();
                }
            }
        }

        /// <summary>
        /// 可空UInt64(ulong?)类型转换器
        /// </summary>
        private class NullableULongConverter : JsonConverter<ulong?>
        {
            public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return null;
                    }
                    if (ulong.TryParse(stringValue, out ulong value))
                    {
                        return value;
                    }
                    throw new JsonException($"无法将字符串值 '{stringValue}' 转换为 ulong?");
                }
                return reader.GetUInt64();
            }

            public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
            {
                if (value.HasValue)
                {
                    writer.WriteStringValue(value.Value.ToString());
                }
                else
                {
                    writer.WriteNullValue();
                }
            }
        }
    }
} 