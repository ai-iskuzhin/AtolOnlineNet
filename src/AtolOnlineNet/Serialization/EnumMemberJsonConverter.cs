using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AtolOnlineNet.Serialization;

/// <summary>
/// Serializes enums to and from the exact wire strings declared with <see cref="EnumMemberAttribute"/>
/// (for example <c>full_payment</c>, <c>vat20</c>, <c>non-operating_gain</c>). Unknown incoming values map to a
/// member named <c>Unknown</c> when present, keeping deserialization forward-compatible as ATOL adds new codes.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public sealed class EnumMemberJsonConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    private static readonly Dictionary<string, T> FromWire = BuildFromWire();
    private static readonly Dictionary<T, string> ToWire = BuildToWire();
    private static readonly bool HasUnknown = Enum.TryParse("Unknown", out T _);

    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        var value = reader.GetString();
        if (value is not null && FromWire.TryGetValue(value, out var result))
        {
            return result;
        }

        if (HasUnknown && Enum.TryParse("Unknown", out T unknown))
        {
            return unknown;
        }

        throw new JsonException($"Unknown {typeof(T).Name} value: '{value}'.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (ToWire.TryGetValue(value, out var wire))
        {
            writer.WriteStringValue(wire);
            return;
        }

        throw new JsonException($"{typeof(T).Name}.{value} has no wire representation.");
    }

    private static Dictionary<string, T> BuildFromWire()
    {
        var map = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var wire = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
            if (wire is not null)
            {
                map[wire] = (T)field.GetValue(null)!;
            }
        }

        return map;
    }

    private static Dictionary<T, string> BuildToWire()
    {
        var map = new Dictionary<T, string>();
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var wire = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
            if (wire is not null)
            {
                map[(T)field.GetValue(null)!] = wire;
            }
        }

        return map;
    }
}
