using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet;

/// <summary>Система налогообложения (тег 1055), поле <c>sno</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<TaxationSystem>))]
public enum TaxationSystem
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Общая СН (<c>osn</c>).</summary>
    [EnumMember(Value = "osn")] Osn,

    /// <summary>Упрощённая СН, доходы (<c>usn_income</c>).</summary>
    [EnumMember(Value = "usn_income")] UsnIncome,

    /// <summary>Упрощённая СН, доходы минус расходы (<c>usn_income_outcome</c>).</summary>
    [EnumMember(Value = "usn_income_outcome")] UsnIncomeOutcome,

    /// <summary>Единый налог на вменённый доход (<c>envd</c>).</summary>
    [EnumMember(Value = "envd")] Envd,

    /// <summary>Единый сельскохозяйственный налог (<c>esn</c>).</summary>
    [EnumMember(Value = "esn")] Esn,

    /// <summary>Патентная СН (<c>patent</c>).</summary>
    [EnumMember(Value = "patent")] Patent,
}

/// <summary>Признак способа расчёта (тег 1214), поле <c>payment_method</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<PaymentMethod>))]
public enum PaymentMethod
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Предоплата 100% (<c>full_prepayment</c>). Значение по умолчанию.</summary>
    [EnumMember(Value = "full_prepayment")] FullPrepayment,

    /// <summary>Предоплата (<c>prepayment</c>).</summary>
    [EnumMember(Value = "prepayment")] Prepayment,

    /// <summary>Аванс (<c>advance</c>).</summary>
    [EnumMember(Value = "advance")] Advance,

    /// <summary>Полный расчёт (<c>full_payment</c>).</summary>
    [EnumMember(Value = "full_payment")] FullPayment,

    /// <summary>Частичный расчёт и кредит (<c>partial_payment</c>).</summary>
    [EnumMember(Value = "partial_payment")] PartialPayment,

    /// <summary>Передача в кредит (<c>credit</c>).</summary>
    [EnumMember(Value = "credit")] Credit,

    /// <summary>Оплата кредита (<c>credit_payment</c>).</summary>
    [EnumMember(Value = "credit_payment")] CreditPayment,
}

/// <summary>Признак предмета расчёта (тег 1212), поле <c>payment_object</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<PaymentObject>))]
public enum PaymentObject
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Товар (<c>commodity</c>). Значение по умолчанию.</summary>
    [EnumMember(Value = "commodity")] Commodity,

    /// <summary>Подакцизный товар (<c>excise</c>).</summary>
    [EnumMember(Value = "excise")] Excise,

    /// <summary>Работа (<c>job</c>).</summary>
    [EnumMember(Value = "job")] Job,

    /// <summary>Услуга (<c>service</c>).</summary>
    [EnumMember(Value = "service")] Service,

    /// <summary>Ставка азартной игры (<c>gambling_bet</c>).</summary>
    [EnumMember(Value = "gambling_bet")] GamblingBet,

    /// <summary>Выигрыш азартной игры (<c>gambling_prize</c>).</summary>
    [EnumMember(Value = "gambling_prize")] GamblingPrize,

    /// <summary>Лотерейный билет (<c>lottery</c>).</summary>
    [EnumMember(Value = "lottery")] Lottery,

    /// <summary>Выигрыш лотереи (<c>lottery_prize</c>).</summary>
    [EnumMember(Value = "lottery_prize")] LotteryPrize,

    /// <summary>Предоставление результатов интеллектуальной деятельности (<c>intellectual_activity</c>).</summary>
    [EnumMember(Value = "intellectual_activity")] IntellectualActivity,

    /// <summary>Платёж (<c>payment</c>).</summary>
    [EnumMember(Value = "payment")] Payment,

    /// <summary>Агентское вознаграждение (<c>agent_commission</c>).</summary>
    [EnumMember(Value = "agent_commission")] AgentCommission,

    /// <summary>Взнос (пеня, штраф, вознаграждение, бонус и т. п.) (<c>award</c>).</summary>
    [EnumMember(Value = "award")] Award,

    /// <summary>Иной предмет расчёта (<c>another</c>).</summary>
    [EnumMember(Value = "another")] Another,

    /// <summary>Имущественное право (<c>property_right</c>).</summary>
    [EnumMember(Value = "property_right")] PropertyRight,

    /// <summary>Внереализационный доход (<c>non-operating_gain</c>).</summary>
    [EnumMember(Value = "non-operating_gain")] NonOperatingGain,

    /// <summary>Страховые взносы (<c>insurance_premium</c>).</summary>
    [EnumMember(Value = "insurance_premium")] InsurancePremium,

    /// <summary>Торговый сбор (<c>sales_tax</c>).</summary>
    [EnumMember(Value = "sales_tax")] SalesTax,

    /// <summary>Залог (<c>deposit</c>).</summary>
    [EnumMember(Value = "deposit")] Deposit,

    /// <summary>Расход (<c>expense</c>).</summary>
    [EnumMember(Value = "expense")] Expense,

    /// <summary>Взносы на ОПС ИП (<c>pension_insurance_ip</c>).</summary>
    [EnumMember(Value = "pension_insurance_ip")] PensionInsuranceIp,

    /// <summary>Взносы на ОПС (<c>pension_insurance</c>).</summary>
    [EnumMember(Value = "pension_insurance")] PensionInsurance,

    /// <summary>Взносы на ОМС ИП (<c>medical_insurance_ip</c>).</summary>
    [EnumMember(Value = "medical_insurance_ip")] MedicalInsuranceIp,

    /// <summary>Взносы на ОМС (<c>medical_insurance</c>).</summary>
    [EnumMember(Value = "medical_insurance")] MedicalInsurance,

    /// <summary>Взносы на ОСС (<c>social_insurance</c>).</summary>
    [EnumMember(Value = "social_insurance")] SocialInsurance,

    /// <summary>Платёж казино (<c>casino_payment</c>).</summary>
    [EnumMember(Value = "casino_payment")] CasinoPayment,

    /// <summary>Туристический налог (<c>resort_fee</c>).</summary>
    [EnumMember(Value = "resort_fee")] ResortFee,
}

/// <summary>Ставка НДС (тег 1199), поле <c>vat.type</c> / <c>vats[].type</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<VatType>))]
public enum VatType
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Без НДС (<c>none</c>).</summary>
    [EnumMember(Value = "none")] None,

    /// <summary>НДС 0% (<c>vat0</c>).</summary>
    [EnumMember(Value = "vat0")] Vat0,

    /// <summary>НДС 10% (<c>vat10</c>).</summary>
    [EnumMember(Value = "vat10")] Vat10,

    /// <summary>НДС 10/110 (<c>vat110</c>).</summary>
    [EnumMember(Value = "vat110")] Vat110,

    /// <summary>НДС 20% (<c>vat20</c>).</summary>
    [EnumMember(Value = "vat20")] Vat20,

    /// <summary>НДС 20/120 (<c>vat120</c>).</summary>
    [EnumMember(Value = "vat120")] Vat120,

    /// <summary>НДС 5% (<c>vat5</c>).</summary>
    [EnumMember(Value = "vat5")] Vat5,

    /// <summary>НДС 7% (<c>vat7</c>).</summary>
    [EnumMember(Value = "vat7")] Vat7,

    /// <summary>НДС 5/105 (<c>vat105</c>).</summary>
    [EnumMember(Value = "vat105")] Vat105,

    /// <summary>НДС 7/107 (<c>vat107</c>).</summary>
    [EnumMember(Value = "vat107")] Vat107,

    /// <summary>НДС 22% (<c>vat22</c>).</summary>
    [EnumMember(Value = "vat22")] Vat22,

    /// <summary>НДС 22/122 (<c>vat122</c>).</summary>
    [EnumMember(Value = "vat122")] Vat122,
}

/// <summary>Признак агента (тег 1057 на чек / 1222 по предмету расчёта), поле <c>agent_info.type</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<AgentType>))]
public enum AgentType
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Банковский платёжный агент (<c>bank_paying_agent</c>).</summary>
    [EnumMember(Value = "bank_paying_agent")] BankPayingAgent,

    /// <summary>Банковский платёжный субагент (<c>bank_paying_subagent</c>).</summary>
    [EnumMember(Value = "bank_paying_subagent")] BankPayingSubagent,

    /// <summary>Платёжный агент (<c>paying_agent</c>).</summary>
    [EnumMember(Value = "paying_agent")] PayingAgent,

    /// <summary>Платёжный субагент (<c>paying_subagent</c>).</summary>
    [EnumMember(Value = "paying_subagent")] PayingSubagent,

    /// <summary>Поверенный (<c>attorney</c>).</summary>
    [EnumMember(Value = "attorney")] Attorney,

    /// <summary>Комиссионер (<c>commission_agent</c>).</summary>
    [EnumMember(Value = "commission_agent")] CommissionAgent,

    /// <summary>Другой тип агента (<c>another</c>).</summary>
    [EnumMember(Value = "another")] Another,
}

/// <summary>Вид оплаты, поле <c>payments[].type</c> (сериализуется числом 0–9).</summary>
public enum PaymentType
{
    /// <summary>Наличные (0).</summary>
    Cash = 0,

    /// <summary>Безналичный (1).</summary>
    Cashless = 1,

    /// <summary>Предварительная оплата, зачёт аванса (2).</summary>
    Prepaid = 2,

    /// <summary>Постоплата, кредит (3).</summary>
    Credit = 3,

    /// <summary>Иная форма оплаты, встречное предоставление (4).</summary>
    Other = 4,

    /// <summary>Расширенный вид оплаты 5.</summary>
    Extended5 = 5,

    /// <summary>Расширенный вид оплаты 6.</summary>
    Extended6 = 6,

    /// <summary>Расширенный вид оплаты 7.</summary>
    Extended7 = 7,

    /// <summary>Расширенный вид оплаты 8.</summary>
    Extended8 = 8,

    /// <summary>Расширенный вид оплаты 9.</summary>
    Extended9 = 9,
}

/// <summary>Тип коррекции (тег 1173), поле <c>correction_info.type</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<CorrectionType>))]
public enum CorrectionType
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Самостоятельно (<c>self</c>).</summary>
    [EnumMember(Value = "self")] Self,

    /// <summary>По предписанию (<c>instruction</c>).</summary>
    [EnumMember(Value = "instruction")] Instruction,
}

/// <summary>Статус обработки документа, поле <c>status</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<DocumentStatus>))]
public enum DocumentStatus
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Ожидание обработки (<c>wait</c>).</summary>
    [EnumMember(Value = "wait")] Wait,

    /// <summary>Документ успешно фискализирован (<c>done</c>).</summary>
    [EnumMember(Value = "done")] Done,

    /// <summary>Ошибка фискализации (<c>fail</c>).</summary>
    [EnumMember(Value = "fail")] Fail,
}

/// <summary>Тип источника ошибки, поле <c>error.type</c>.</summary>
[JsonConverter(typeof(EnumMemberJsonConverter<AtolErrorType>))]
public enum AtolErrorType
{
    /// <summary>Неизвестное/нераспознанное значение.</summary>
    Unknown = 0,

    /// <summary>Отсутствие ошибки (<c>none</c>).</summary>
    [EnumMember(Value = "none")] None,

    /// <summary>Системная ошибка (<c>system</c>).</summary>
    [EnumMember(Value = "system")] System,

    /// <summary>Ошибка при работе с ККТ (<c>driver</c>).</summary>
    [EnumMember(Value = "driver")] Driver,

    /// <summary>Превышено время ожидания (<c>timeout</c>).</summary>
    [EnumMember(Value = "timeout")] Timeout,

    /// <summary>Ошибка агента (<c>agent</c>).</summary>
    [EnumMember(Value = "agent")] Agent,
}

/// <summary>Операция регистрации чека прихода/расхода/возврата (сегмент URL).</summary>
public enum ReceiptOperation
{
    /// <summary>Чек «Приход» (<c>sell</c>).</summary>
    Sell,

    /// <summary>Чек «Возврат прихода» (<c>sell_refund</c>).</summary>
    SellRefund,

    /// <summary>Чек «Расход» (<c>buy</c>).</summary>
    Buy,

    /// <summary>Чек «Возврат расхода» (<c>buy_refund</c>).</summary>
    BuyRefund,
}

/// <summary>Операция регистрации чека коррекции (сегмент URL).</summary>
public enum CorrectionOperation
{
    /// <summary>Чек «Коррекция прихода» (<c>sell_correction</c>).</summary>
    SellCorrection,

    /// <summary>Чек «Коррекция расхода» (<c>buy_correction</c>).</summary>
    BuyCorrection,
}

internal static class OperationExtensions
{
    public static string ToWire(this ReceiptOperation operation) => operation switch
    {
        ReceiptOperation.Sell => "sell",
        ReceiptOperation.SellRefund => "sell_refund",
        ReceiptOperation.Buy => "buy",
        ReceiptOperation.BuyRefund => "buy_refund",
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };

    public static string ToWire(this CorrectionOperation operation) => operation switch
    {
        CorrectionOperation.SellCorrection => "sell_correction",
        CorrectionOperation.BuyCorrection => "buy_correction",
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };
}
