using System.Collections.Generic;

namespace AtolOnlineNet.V1_05;

/// <summary>Атрибуты агента (<c>agent_info</c>), на чек (тег 1057) или на предмет расчёта (тег 1222).</summary>
public sealed record AgentInfo
{
    /// <summary>Признак агента. Обязателен, если передан <see cref="AgentInfo"/>.</summary>
    public AgentType? Type { get; init; }

    /// <summary>Атрибуты платёжного агента.</summary>
    public PayingAgent? PayingAgent { get; init; }

    /// <summary>Атрибуты оператора по приёму платежей.</summary>
    public ReceivePaymentsOperator? ReceivePaymentsOperator { get; init; }

    /// <summary>Атрибуты оператора перевода.</summary>
    public MoneyTransferOperator? MoneyTransferOperator { get; init; }
}

/// <summary>Атрибуты платёжного агента (<c>paying_agent</c>).</summary>
public sealed record PayingAgent
{
    /// <summary>Наименование операции (тег 1044). Макс. 24 символа.</summary>
    public string? Operation { get; init; }

    /// <summary>Телефоны платёжного агента (тег 1073).</summary>
    public IReadOnlyList<string>? Phones { get; init; }
}

/// <summary>Атрибуты оператора по приёму платежей (<c>receive_payments_operator</c>).</summary>
public sealed record ReceivePaymentsOperator
{
    /// <summary>Телефоны оператора по приёму платежей (тег 1074).</summary>
    public IReadOnlyList<string>? Phones { get; init; }
}

/// <summary>Атрибуты оператора перевода (<c>money_transfer_operator</c>).</summary>
public sealed record MoneyTransferOperator
{
    /// <summary>Телефоны оператора перевода (тег 1075).</summary>
    public IReadOnlyList<string>? Phones { get; init; }

    /// <summary>Наименование оператора перевода (тег 1026). Макс. 64 символа.</summary>
    public string? Name { get; init; }

    /// <summary>Адрес оператора перевода (тег 1005). Макс. 256 символов.</summary>
    public string? Address { get; init; }

    /// <summary>ИНН оператора перевода (тег 1016). Макс. 12 символов.</summary>
    public string? Inn { get; init; }
}

/// <summary>Атрибуты поставщика (<c>supplier_info</c>). Обязателен, если передан <c>agent_info</c>.</summary>
public sealed record SupplierInfo
{
    /// <summary>Телефоны поставщика (тег 1171).</summary>
    public IReadOnlyList<string>? Phones { get; init; }

    /// <summary>Наименование поставщика (тег 1225).</summary>
    public string? Name { get; init; }

    /// <summary>ИНН поставщика (тег 1226). Обязателен, если передан <see cref="SupplierInfo"/>.</summary>
    public string? Inn { get; init; }
}
