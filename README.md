<table>
  <tr>
    <td width="170" align="center" valign="middle">
      <img src="https://raw.githubusercontent.com/ai-iskuzhin/AtolOnlineNet/main/assets/icon.png" width="140" alt="AtolOnlineNet logo" />
    </td>
    <td valign="middle">
      <h1>AtolOnlineNet</h1>
      <p>.NET SDK для облачной фискализации <a href="https://online.atol.ru/">АТОЛ Онлайн</a> (сервис <code>possystem</code> v4, ФФД 1.05): авторизация, регистрация чеков прихода/расхода/возврата, чеков коррекции и получение результата фискализации.</p>
      <p>
        <a href="https://github.com/ai-iskuzhin/AtolOnlineNet/actions/workflows/ci.yml"><img src="https://github.com/ai-iskuzhin/AtolOnlineNet/actions/workflows/ci.yml/badge.svg?branch=main" alt="CI" /></a>
        <a href="https://github.com/ai-iskuzhin/AtolOnlineNet/actions/workflows/release.yml"><img src="https://github.com/ai-iskuzhin/AtolOnlineNet/actions/workflows/release.yml/badge.svg" alt="Release" /></a>
        <a href="https://github.com/ai-iskuzhin/AtolOnlineNet/blob/main/LICENSE"><img src="https://img.shields.io/github/license/ai-iskuzhin/AtolOnlineNet?style=flat-square" alt="License" /></a>
        <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/targets-netstandard2.0%20%7C%20net8.0%20%7C%20net10.0-512BD4?logo=dotnet&amp;style=flat-square" alt="Targets" /></a>
      </p>
      <p>
        <a href="https://www.nuget.org/packages/AtolOnlineNet"><img src="https://img.shields.io/nuget/v/AtolOnlineNet?logo=nuget&amp;style=flat-square" alt="NuGet version" /></a>
        <a href="https://www.nuget.org/packages/AtolOnlineNet"><img src="https://img.shields.io/nuget/dt/AtolOnlineNet?style=flat-square" alt="NuGet downloads" /></a>
      </p>
    </td>
  </tr>
</table>

> SDK работает с облачным API
> регистрации чеков (`/possystem/v4/...`). Версия `v5` (ФФД 1.2) пока не реализована — см. [Roadmap](#roadmap).

## Установка

```bash
dotnet add package AtolOnlineNet
```

Целевые платформы: `netstandard2.0`, `net8.0`, `net10.0` (работает на .NET Framework 4.6.1+, .NET Core 2.0+, .NET 8/10).

## Быстрый старт

```csharp
using AtolOnlineNet;
using AtolOnlineNet.V1_05;

var options = new AtolOnlineClientOptions
{
    Login = "your-login",
    Password = "your-password",
    GroupCode = "your-group-code",
    BaseAddress = AtolOnlineClientOptions.TestBaseAddress, // или ProductionBaseAddress
};

using var client = new AtolOnlineClient(options); // или new AtolOnlineClient(httpClient, options)

// Токен запрашивается автоматически и кешируется на 24 часа.
var registered = await client.SellAsync(new ReceiptRegistrationRequest
{
    ExternalId = Guid.NewGuid().ToString("N"),
    Timestamp = DateTime.Now,
    Receipt = new Receipt
    {
        Client = new Client { Email = "buyer@example.com" },
        Company = new Company
        {
            Email = "shop@example.com",
            Sno = TaxationSystem.Osn,
            Inn = "5544332219",
            PaymentAddress = "https://shop.example.com",
        },
        Items =
        [
            new Item
            {
                Name = "Носки хлопковые",
                Price = 199.00m,
                Quantity = 2m,
                Sum = 398.00m,
                PaymentMethod = PaymentMethod.FullPayment,
                PaymentObject = PaymentObject.Commodity,
                Vat = new Vat { Type = VatType.Vat20 },
            },
        ],
        Payments = [new Payment { Type = PaymentType.Cashless, Sum = 398.00m }],
        Total = 398.00m,
    },
});

Console.WriteLine($"UUID: {registered.Uuid}, статус: {registered.Status}");

// Получение результата по UUID (опрос до терминального статуса).
var report = await client.GetReportAsync(registered.Uuid!);
if (report.Status == DocumentStatus.Done)
{
    Console.WriteLine($"ФД №{report.Payload!.FiscalDocumentNumber}, ФПД {report.Payload.FiscalDocumentAttribute}");
    Console.WriteLine($"Чек ОФД: {report.Payload.OfdReceiptUrl}");
}
```

## Среды

| Среда | Базовый адрес |
|-------|---------------|
| Тестовая (ФФД 1.05) | `AtolOnlineClientOptions.TestBaseAddress` → `https://testonline.atol.ru/possystem/` |
| Боевая | `AtolOnlineClientOptions.ProductionBaseAddress` → `https://online.atol.ru/possystem/` |

## Поддерживаемые методы

| Метод | Операция | Описание |
|-------|----------|----------|
| `GetTokenAsync` | `getToken` | Авторизация, получение токена (кешируется на 24 ч). |
| `SellAsync` | `sell` | Чек «Приход». |
| `SellRefundAsync` | `sell_refund` | Чек «Возврат прихода». |
| `BuyAsync` | `buy` | Чек «Расход». |
| `BuyRefundAsync` | `buy_refund` | Чек «Возврат расхода». |
| `SellCorrectionAsync` | `sell_correction` | Чек «Коррекция прихода». |
| `BuyCorrectionAsync` | `buy_correction` | Чек «Коррекция расхода». |
| `GetReportAsync` | `report/{uuid}` | Результат обработки документа. |

Сам `AtolOnlineClient` версионно-нейтрален: транспорт (токен / регистрация / результат) одинаков для
всех версий ФФД. Типобезопасные методы `SellAsync`/`BuyAsync`/… — это методы-расширения из пространства
имён **`AtolOnlineNet.V1_05`** (ФФД 1.05), работающие с моделями чека этой версии. Для динамического
выбора операции есть нейтральные `RegisterAsync(ReceiptOperation, object)` и
`RegisterCorrectionAsync(CorrectionOperation, object)`. Модели чека ФФД 1.2 будут добавлены в
`AtolOnlineNet.V1_2`; версия выбирается через `AtolOnlineClientOptions.ApiVersion`.

К каждому запросу автоматически добавляется заголовок `User-Agent` вида `AtolOnlineNet/<версия>`,
если на `HttpClient` не задан собственный.

## Чек коррекции

```csharp
var correction = await client.SellCorrectionAsync(new CorrectionRegistrationRequest
{
    ExternalId = Guid.NewGuid().ToString("N"),
    Timestamp = DateTime.Now,
    Correction = new Correction
    {
        Company = new Company { Email = "shop@example.com", Sno = TaxationSystem.Osn, Inn = "5544332219", PaymentAddress = "https://shop.example.com" },
        CorrectionInfo = new CorrectionInfo
        {
            Type = CorrectionType.Self,
            BaseDate = new DateTime(2026, 6, 1),
        },
        Payments = [new Payment { Type = PaymentType.Cashless, Sum = 100m }],
        Vats = [new Vat { Type = VatType.Vat20, Sum = 16.67m }],
    },
});
```

## Обработка ошибок

Бизнес-результат возвращается в объектах ответа — статус (`Wait` / `Done` / `Fail`) и `Error`
не выбрасываются как исключения, их нужно проверять:

```csharp
var report = await client.GetReportAsync(uuid);
switch (report.Status)
{
    case DocumentStatus.Done: /* реквизиты в report.Payload */ break;
    case DocumentStatus.Wait: /* ещё обрабатывается — повторить позже */ break;
    case DocumentStatus.Fail: Console.WriteLine($"Ошибка {report.Error?.Code}: {report.Error?.Text}"); break;
}
```

Исключения выбрасываются только в исключительных ситуациях:

| Исключение | Когда |
|------------|-------|
| `AtolOnlineApiException` | Ошибка авторизации (`getToken` вернул `error`). Содержит `Error` (код, текст, тип). |
| `AtolOnlineTransportException` | Сетевая ошибка / таймаут — ответ не получен. |
| `AtolOnlineProtocolException` | Получен непарсируемый ответ (не JSON, пустое тело). Содержит статус и превью тела. |
| `AtolOnlineUnknownEnumValueException` | ATOL вернул значение enum, неизвестное этой версии SDK (как правило, добавленное уже после релиза пакета). Это пробел в SDK — сообщите о нём: <https://github.com/ai-iskuzhin/AtolOnlineNet/issues/new>. |

Символьные имена кодов ошибок сервиса — в `AtolServiceErrorCodes` (например `ExpiredToken = 11`).
Истёкший токен обрабатывается автоматически (повторная авторизация и один повтор запроса).

## Внедрение зависимостей

Ядро не зависит от `Microsoft.Extensions.*`. Регистрация через `IHttpClientFactory`:

```csharp
services.AddHttpClient("atol-online");
services.AddSingleton<IAtolOnlineClient>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("atol-online");
    return new AtolOnlineClient(http, new AtolOnlineClientOptions
    {
        Login = "...",
        Password = "...",
        GroupCode = "...",
        BaseAddress = AtolOnlineClientOptions.ProductionBaseAddress,
    });
});
```

## Разработка

```bash
dotnet build AtolOnlineNet.slnx -c Release
dotnet test  tests/AtolOnlineNet.Tests/AtolOnlineNet.Tests.csproj -c Release   # модульные тесты
dotnet pack  src/AtolOnlineNet/AtolOnlineNet.csproj -c Release -o artifacts/packages
```

Интеграционные тесты (`tests/AtolOnlineNet.Tests.Integration`) выполняются против тестовой среды АТОЛ
только при заданных учётных данных — скопируйте `.env.example` в `.env`. Без учётных данных они
пропускаются.

## Документация

- [Протокол фискализации v4 (ФФД 1.05)](docs/atol-online-fiscalization-v4-ffd-1.05-api-spec.md)
- [Сервис информации о ККТ и ФН (отдельный API)](docs/atol-online-kkt-info-api-spec.md)
- [Соглашения проекта](AGENTS.md) · [История изменений](CHANGELOG.md)

## Roadmap

- Поддержка `possystem` **v5** (ФФД 1.2). Транспорт (токен/регистрация/результат) общий; добавляются
  только модели чека ФФД 1.2, версия выбирается через `AtolOnlineClientOptions.ApiVersion`.

## Лицензия

[MIT](LICENSE)
