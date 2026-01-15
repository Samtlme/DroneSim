using StackExchange.Redis;
using System.Text.Json;

namespace DroneSim.Infrastructure.Extensions
{
    internal static class RedisValueJsonExtensions
    {
        /// <summary>
        /// Deserializes a RedisValue assumed to contain a JSON string.
        /// This exists to avoid ambiguous JsonSerializer overloads introduced in .NET 10 between string and ReadOnlySpan<byte>
        /// </summary>
        public static T DeserializeJson<T>(this RedisValue value, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize<T>((string)value!, options)!;
        }
    }
}
