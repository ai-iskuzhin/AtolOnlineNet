using System.Text.Json.Serialization;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet.V1_05;

/// <summary>
/// Запрос регистрации чека прихода/расхода/возврата (тело POST <c>/{group_code}/{operation}</c> для
/// <c>sell</c>, <c>sell_refund</c>, <c>buy</c>, <c>buy_refund</c>).
/// </summary>
public sealed record ReceiptRegistrationRequest
{
    /// <summary>
    /// Идентификатор документа внешней системы, уникальный в рамках группы ККТ. Макс. 128 символов.
    /// Защищает от потери документа при разрывах связи: повторная подача с тем же <see cref="ExternalId"/>
    /// вернёт ранее присвоенный UUID.
    /// </summary>
    public required string ExternalId { get; init; }

    /// <summary>Дата и время документа внешней системы, формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public required DateTime Timestamp { get; init; }

    /// <summary>Чек.</summary>
    public required Receipt Receipt { get; init; }

    /// <summary>Служебный раздел (например, <c>callback_url</c>).</summary>
    public Service? Service { get; init; }
}

/// <summary>
/// Запрос регистрации чека коррекции (тело POST <c>/{group_code}/{operation}</c> для
/// <c>sell_correction</c>, <c>buy_correction</c>).
/// </summary>
public sealed record CorrectionRegistrationRequest
{
    /// <summary>Идентификатор документа внешней системы, уникальный в рамках группы ККТ. Макс. 128 символов.</summary>
    public required string ExternalId { get; init; }

    /// <summary>Дата и время документа внешней системы, формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public required DateTime Timestamp { get; init; }

    /// <summary>Чек коррекции.</summary>
    public required Correction Correction { get; init; }

    /// <summary>Служебный раздел (например, <c>callback_url</c>).</summary>
    public Service? Service { get; init; }
}
