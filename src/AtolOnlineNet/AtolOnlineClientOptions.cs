namespace AtolOnlineNet;

/// <summary>Configuration for <see cref="AtolOnlineClient"/>.</summary>
public sealed class AtolOnlineClientOptions
{
    /// <summary>Default production base address for the <c>possystem</c> service.</summary>
    public static readonly Uri ProductionBaseAddress = new("https://online.atol.ru/possystem/");

    /// <summary>Default test base address for the <c>possystem</c> service (FFD 1.05 / v4).</summary>
    public static readonly Uri TestBaseAddress = new("https://testonline.atol.ru/possystem/");

    /// <summary>Логин пользователя для отправки данных (из настроек CMS). Макс. 100 символов.</summary>
    public required string Login { get; init; }

    /// <summary>Пароль пользователя для отправки данных (из настроек CMS).</summary>
    public required string Password { get; init; }

    /// <summary>Идентификатор группы ККТ. Требуется для регистрации документов и получения результата.</summary>
    public string? GroupCode { get; init; }

    /// <summary>Наименование интегратора (необязательный параметр <c>source</c>). Макс. 100 символов.</summary>
    public string? Source { get; init; }

    /// <summary>Версия API сервиса. По умолчанию <c>v4</c> (ФФД 1.05).</summary>
    public string ApiVersion { get; init; } = "v4";

    /// <summary>
    /// Базовый адрес сервиса (должен оканчиваться на <c>/possystem/</c>). По умолчанию
    /// <see cref="ProductionBaseAddress"/>.
    /// </summary>
    public Uri BaseAddress { get; init; } = ProductionBaseAddress;
}
