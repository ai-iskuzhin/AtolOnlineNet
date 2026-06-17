namespace AtolOnlineNet.V1_05;

/// <summary>Служебный раздел запроса (<c>service</c>).</summary>
public sealed record Service
{
    /// <summary>URL, на который сервис отправит результат обработки документа (тег —). Макс. 256 символов.</summary>
    public string? CallbackUrl { get; init; }
}

/// <summary>Атрибуты клиента (покупателя) (<c>receipt.client</c>).</summary>
public sealed record Client
{
    /// <summary>Электронная почта покупателя (тег 1008). Макс. 64 символа. Заполните <see cref="Email"/> или <see cref="Phone"/>.</summary>
    public string? Email { get; init; }

    /// <summary>Телефон покупателя (тег 1008). С кодом страны без пробелов, кроме «+». Макс. 64 символа.</summary>
    public string? Phone { get; init; }

    /// <summary>Наименование покупателя (тег 1227). Макс. 256 символов.</summary>
    public string? Name { get; init; }

    /// <summary>ИНН покупателя (тег 1228). 10 или 12 цифр.</summary>
    public string? Inn { get; init; }
}

/// <summary>Атрибуты компании (<c>receipt.company</c>).</summary>
public sealed record Company
{
    /// <summary>Электронная почта отправителя чека (тег 1117). Макс. 64 символа.</summary>
    public required string Email { get; init; }

    /// <summary>Система налогообложения (тег 1055). Необязательно, если у организации один тип СН.</summary>
    public TaxationSystem? Sno { get; init; }

    /// <summary>ИНН организации (тег 1018). 10 или 12 цифр. Сравнивается со значением в ФН.</summary>
    public required string Inn { get; init; }

    /// <summary>Место расчётов (тег 1187). Макс. 256 символов.</summary>
    public required string PaymentAddress { get; init; }

    /// <summary>Адрес расчётов (тег 1009). 1–256 символов. При отсутствии будет указан адрес ЦОД.</summary>
    public string? Location { get; init; }
}
