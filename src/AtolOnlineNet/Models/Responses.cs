using System.Text.Json.Serialization;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet.Models;

/// <summary>Ответ на запрос авторизации (<c>getToken</c>).</summary>
public sealed record TokenResponse
{
    /// <summary>Авторизационный токен. Макс. 1000 символов. Возвращается только при отсутствии ошибки.</summary>
    public string? Token { get; init; }

    /// <summary>Ошибка, если авторизация не удалась.</summary>
    public AtolError? Error { get; init; }

    /// <summary>Дата и время ответа, формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public DateTime Timestamp { get; init; }
}

/// <summary>Ответ на POST-запрос регистрации документа.</summary>
public sealed record RegisterDocumentResponse
{
    /// <summary>Уникальный идентификатор документа. Макс. 128 символов. Не присваивается при ошибке регистрации.</summary>
    public string? Uuid { get; init; }

    /// <summary>Дата и время ответа, формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public DateTime Timestamp { get; init; }

    /// <summary>Статус обработки (<c>wait</c> / <c>fail</c>).</summary>
    public DocumentStatus Status { get; init; }

    /// <summary>Ошибка, если регистрация не удалась.</summary>
    public AtolError? Error { get; init; }
}

/// <summary>Важная информация, сопровождающая результат (<c>warnings</c>).</summary>
public sealed record ReportWarnings
{
    /// <summary>Имеет значение, если <c>callback_url</c> в запросе не соответствует маске.</summary>
    public string? CallbackUrl { get; init; }
}

/// <summary>Реквизиты фискализации документа (<c>payload</c>).</summary>
public sealed record ReportPayload
{
    /// <summary>Номер чека в смене (тег 1042).</summary>
    public int FiscalReceiptNumber { get; init; }

    /// <summary>Номер смены (тег 1038).</summary>
    public int ShiftNumber { get; init; }

    /// <summary>Дата и время документа из ФН (тег 1012), формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public DateTime ReceiptDatetime { get; init; }

    /// <summary>Итоговая сумма документа (тег 1020) в рублях.</summary>
    public decimal Total { get; init; }

    /// <summary>Номер ФН (тег 1041).</summary>
    public string? FnNumber { get; init; }

    /// <summary>Регистрационный номер ККТ (тег 1037).</summary>
    public string? EcrRegistrationNumber { get; init; }

    /// <summary>Фискальный номер документа (тег 1040).</summary>
    public long FiscalDocumentNumber { get; init; }

    /// <summary>Фискальный признак документа, ФПД (тег 1077).</summary>
    public long FiscalDocumentAttribute { get; init; }

    /// <summary>Адрес сайта ФНС (тег 1060).</summary>
    public string? FnsSite { get; init; }

    /// <summary>ИНН оператора фискальных данных (тег 1017).</summary>
    public string? OfdInn { get; init; }

    /// <summary>URL для просмотра чека на сайте ОФД (для поддерживаемых ОФД).</summary>
    public string? OfdReceiptUrl { get; init; }
}

/// <summary>Пакет результата обработки документа (GET <c>/report/{uuid}</c> или callback).</summary>
public sealed record DocumentReportResponse
{
    /// <summary>Уникальный идентификатор документа. Макс. 128 символов.</summary>
    public string? Uuid { get; init; }

    /// <summary>Дата и время ответа, формат <c>dd.MM.yyyy HH:mm:ss</c>.</summary>
    [JsonConverter(typeof(AtolDateTimeJsonConverter))]
    public DateTime Timestamp { get; init; }

    /// <summary>Статус обработки (<c>done</c> / <c>fail</c> / <c>wait</c>).</summary>
    public DocumentStatus Status { get; init; }

    /// <summary>URL обратного вызова, указанный при регистрации.</summary>
    public string? CallbackUrl { get; init; }

    /// <summary>Идентификатор группы ККТ.</summary>
    public string? GroupCode { get; init; }

    /// <summary>Наименование сервера.</summary>
    public string? DaemonCode { get; init; }

    /// <summary>Код ККТ.</summary>
    public string? DeviceCode { get; init; }

    /// <summary>Идентификатор документа внешней системы.</summary>
    public string? ExternalId { get; init; }

    /// <summary>Ошибка, если фискализация не удалась или ещё не завершена.</summary>
    public AtolError? Error { get; init; }

    /// <summary>Важная информация (например, о некорректном <c>callback_url</c>).</summary>
    public ReportWarnings? Warnings { get; init; }

    /// <summary>Реквизиты фискализации. Заполнен при <see cref="DocumentStatus.Done"/>.</summary>
    public ReportPayload? Payload { get; init; }
}
