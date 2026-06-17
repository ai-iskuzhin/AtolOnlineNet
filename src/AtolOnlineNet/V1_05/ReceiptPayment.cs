namespace AtolOnlineNet.V1_05;

/// <summary>Оплата (<c>payments[]</c>).</summary>
public sealed record Payment
{
    /// <summary>Вид оплаты (число 0–9).</summary>
    public required PaymentType Type { get; init; }

    /// <summary>Сумма к оплате в рублях. Если не передана, будет указана итоговая сумма чека с видом «Безналичный».</summary>
    public decimal? Sum { get; init; }
}

/// <summary>Сведения об оплате в безналичном порядке (<c>cashless_payments[]</c>, тег 1235).</summary>
public sealed record CashlessPayment
{
    /// <summary>Сумма оплаты безналичными (тег 1082) в рублях.</summary>
    public required decimal Sum { get; init; }

    /// <summary>Признак способа оплаты безналичными (тег 1236). Целое 0–255.</summary>
    public required int Method { get; init; }

    /// <summary>Идентификатор безналичной оплаты (тег 1237). Макс. 256 символов.</summary>
    public required string Id { get; init; }

    /// <summary>Дополнительные сведения о безналичной оплате (тег 1238). Макс. 256 символов.</summary>
    public string? AdditionalInfo { get; init; }
}

/// <summary>Дополнительный реквизит пользователя (<c>additional_user_props</c>, тег 1084).</summary>
public sealed record AdditionalUserProps
{
    /// <summary>Наименование доп. реквизита пользователя (тег 1085). Макс. 64 символа.</summary>
    public required string Name { get; init; }

    /// <summary>Значение доп. реквизита пользователя (тег 1086). Макс. 256 символов.</summary>
    public required string Value { get; init; }
}
