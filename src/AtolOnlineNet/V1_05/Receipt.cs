using System.Collections.Generic;

namespace AtolOnlineNet.V1_05;

/// <summary>
/// Чек прихода/расхода/возврата (<c>receipt</c>) — тело документов <c>sell</c>, <c>sell_refund</c>,
/// <c>buy</c>, <c>buy_refund</c>.
/// </summary>
public sealed record Receipt
{
    /// <summary>Атрибуты клиента (покупателя). Обязательно: email или phone.</summary>
    public required Client Client { get; init; }

    /// <summary>Атрибуты компании.</summary>
    public required Company Company { get; init; }

    /// <summary>Атрибуты агента на чек.</summary>
    public AgentInfo? AgentInfo { get; init; }

    /// <summary>Атрибуты поставщика на чек. Обязателен, если передан <see cref="AgentInfo"/>.</summary>
    public SupplierInfo? SupplierInfo { get; init; }

    /// <summary>Позиции чека. От 1 до 100 элементов.</summary>
    public required IReadOnlyList<Item> Items { get; init; }

    /// <summary>Оплаты. От 1 до 10 элементов.</summary>
    public required IReadOnlyList<Payment> Payments { get; init; }

    /// <summary>Налоги на чек. От 1 до 6 элементов. Передаётся либо налог на позицию, либо на чек.</summary>
    public IReadOnlyList<Vat>? Vats { get; init; }

    /// <summary>Итоговая сумма чека (тег 1020) в рублях.</summary>
    public required decimal Total { get; init; }

    /// <summary>Дополнительный реквизит чека (тег 1192). Макс. 16 символов.</summary>
    public string? AdditionalCheckProps { get; init; }

    /// <summary>ФИО кассира (тег 1021). Макс. 64 символа.</summary>
    public string? Cashier { get; init; }

    /// <summary>Дополнительный реквизит пользователя (тег 1084).</summary>
    public AdditionalUserProps? AdditionalUserProps { get; init; }

    /// <summary>Заводской номер автоматического устройства для расчётов (тег 1036). 1–20 символов.</summary>
    public string? DeviceNumber { get; init; }

    /// <summary>Признак расчёта в «Интернет» (тег 1125).</summary>
    public bool? Internet { get; init; }

    /// <summary>Сведения об оплате в безналичном порядке (тег 1235).</summary>
    public IReadOnlyList<CashlessPayment>? CashlessPayments { get; init; }
}
