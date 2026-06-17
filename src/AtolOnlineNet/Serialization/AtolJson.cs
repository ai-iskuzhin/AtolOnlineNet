using System.Text.Json;
using System.Text.Json.Serialization;

namespace AtolOnlineNet.Serialization;

/// <summary>
/// Centralized <see cref="JsonSerializerOptions"/> for the ATOL Online wire protocol: <c>snake_case</c>
/// property names, omission of <see langword="null"/> values, and non-escaping of the Cyrillic text that
/// pervades receipts.
/// </summary>
public static class AtolJson
{
    /// <summary>The shared, read-only serializer options used by the SDK.</summary>
    public static readonly JsonSerializerOptions Options = Create();

    /// <summary>Creates a fresh, writable copy of the SDK serializer options.</summary>
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Receipt text is Russian; do not escape it into \uXXXX sequences.
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        return options;
    }
}
