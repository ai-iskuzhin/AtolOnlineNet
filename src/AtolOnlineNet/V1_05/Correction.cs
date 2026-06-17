using System.Collections.Generic;
using System.Text.Json.Serialization;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet.V1_05;

/// <summary>Сведения о коррекции (<c>correction_info</c>).</summary>
public sealed record CorrectionInfo
{
    /// <summary>Тип коррекции (тег 1173).</summary>
    public required CorrectionType Type { get; init; }

    /// <summary>Дата совершения корректируемого расчёта (тег 1178), формат <c>dd.MM.yyyy</c>.</summary>
    [JsonConverter(typeof(AtolDateJsonConverter))]
    public required DateTime BaseDate { get; init; }

    /// <summary>Номер предписания налогового органа (тег 1179). Обязателен при <see cref="CorrectionType.Instruction"/>.</summary>
    public string? BaseNumber { get; init; }
}

/// <summary>
/// Чек коррекции (<c>correction</c>) — тело документов <c>sell_correction</c> и <c>buy_correction</c>.
/// </summary>
public sealed record Correction
{
    /// <summary>Атрибуты компании.</summary>
    public required Company Company { get; init; }

    /// <summary>Сведения о коррекции.</summary>
    public required CorrectionInfo CorrectionInfo { get; init; }

    /// <summary>Атрибуты клиента. Обязателен при <see cref="Internet"/> = <see langword="true"/>.</summary>
    public Client? Client { get; init; }

    /// <summary>Оплаты. От 1 до 10 элементов.</summary>
    public required IReadOnlyList<Payment> Payments { get; init; }

    /// <summary>Налоги на чек коррекции. От 1 до 6 элементов.</summary>
    public required IReadOnlyList<Vat> Vats { get; init; }

    /// <summary>ФИО кассира (тег 1021). Макс. 64 символа.</summary>
    public string? Cashier { get; init; }

    /// <summary>Заводской номер устройства (тег 1036). 1–20 символов.</summary>
    public string? DeviceNumber { get; init; }

    /// <summary>Признак расчёта в «Интернет» (тег 1125).</summary>
    public bool? Internet { get; init; }
}
