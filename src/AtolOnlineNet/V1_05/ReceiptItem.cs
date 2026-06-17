namespace AtolOnlineNet.V1_05;

/// <summary>Атрибуты налога (<c>vat</c>).</summary>
public sealed record Vat
{
    /// <summary>Ставка НДС (тег 1199).</summary>
    public required VatType Type { get; init; }

    /// <summary>Сумма налога (тег 1200) в рублях. Целая часть ≤ 11 знаков, дробная ≤ 2.</summary>
    public decimal? Sum { get; init; }
}

/// <summary>Позиция чека (<c>receipt.items[]</c>).</summary>
public sealed record Item
{
    /// <summary>Наименование предмета расчёта (тег 1030). Макс. 128 символов.</summary>
    public required string Name { get; init; }

    /// <summary>Цена за единицу с учётом скидок и наценок (тег 1079) в рублях.</summary>
    public required decimal Price { get; init; }

    /// <summary>Количество/вес предмета расчёта (тег 1023). От 0.000001 до 99999999.</summary>
    public required decimal Quantity { get; init; }

    /// <summary>Стоимость предмета расчёта с учётом скидок и наценок (тег 1043) в рублях.</summary>
    public required decimal Sum { get; init; }

    /// <summary>Единица измерения (тег 1197). Макс. 16 символов.</summary>
    public string? MeasurementUnit { get; init; }

    /// <summary>Код товара (тег 1162): hex-представление с пробелами либо код GS1 Data Matrix.</summary>
    public string? NomenclatureCode { get; init; }

    /// <summary>Признак способа расчёта (тег 1214). По умолчанию <see cref="PaymentMethod.FullPrepayment"/>.</summary>
    public PaymentMethod? PaymentMethod { get; init; }

    /// <summary>Признак предмета расчёта (тег 1212). По умолчанию <see cref="PaymentObject.Commodity"/>.</summary>
    public PaymentObject? PaymentObject { get; init; }

    /// <summary>Атрибуты налога на позицию.</summary>
    public required Vat Vat { get; init; }

    /// <summary>Атрибуты агента по предмету расчёта.</summary>
    public AgentInfo? AgentInfo { get; init; }

    /// <summary>Атрибуты поставщика по предмету расчёта.</summary>
    public SupplierInfo? SupplierInfo { get; init; }

    /// <summary>Дополнительный реквизит предмета расчёта (тег 1191). Макс. 64 символа.</summary>
    public string? UserData { get; init; }

    /// <summary>Сумма акциза (тег 1229) в рублях. Не может быть отрицательной.</summary>
    public decimal? Excise { get; init; }

    /// <summary>Цифровой код страны происхождения товара (тег 1230). 3 цифры.</summary>
    public string? CountryCode { get; init; }

    /// <summary>Номер таможенной декларации (тег 1231). Макс. 32 символа.</summary>
    public string? DeclarationNumber { get; init; }
}
