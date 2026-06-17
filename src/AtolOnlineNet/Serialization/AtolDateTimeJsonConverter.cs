using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AtolOnlineNet.Serialization;

/// <summary>
/// Serializes <see cref="DateTime"/> values using ATOL's <c>dd.MM.yyyy HH:mm:ss</c> wire format
/// (used by <c>timestamp</c> and <c>receipt_datetime</c> fields).
/// </summary>
public sealed class AtolDateTimeJsonConverter : JsonConverter<DateTime>
{
    /// <summary>The ATOL date-time format.</summary>
    public const string Format = "dd.MM.yyyy HH:mm:ss";

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateTime.ParseExact(value!, Format, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}

/// <summary>
/// Serializes <see cref="DateTime"/> values using ATOL's date-only <c>dd.MM.yyyy</c> wire format
/// (used by the correction <c>base_date</c> field).
/// </summary>
public sealed class AtolDateJsonConverter : JsonConverter<DateTime>
{
    /// <summary>The ATOL date-only format.</summary>
    public const string Format = "dd.MM.yyyy";

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateTime.ParseExact(value!, Format, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
}
