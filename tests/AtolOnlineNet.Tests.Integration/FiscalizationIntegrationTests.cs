using System.Diagnostics;
using AtolOnlineNet;
using AtolOnlineNet.Models;
using AtolOnlineNet.V1_05;
using Xunit.Abstractions;

namespace AtolOnlineNet.Tests.Integration;

/// <summary>
/// Live tests against the ATOL Online test environment (<c>https://testonline.atol.ru/possystem/v4/</c>).
/// They run only when credentials are configured (see <c>.env.example</c>); otherwise they short-circuit.
/// </summary>
public class FiscalizationIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public FiscalizationIntegrationTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public async Task GetToken_ReturnsTokenFromTestEnvironment()
    {
        if (!IntegrationConfig.IsConfigured)
        {
            _output.WriteLine("Integration credentials not configured; skipping.");
            return;
        }

        using var client = new AtolOnlineClient(IntegrationConfig.CreateOptions());

        var token = await client.GetTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(token.Token));
        Assert.Null(token.Error);
        _output.WriteLine($"Token issued at {token.Timestamp:O}");
    }

    [Fact]
    public async Task RegisterSell_ThenPollReport_ReachesTerminalStatus()
    {
        if (!IntegrationConfig.IsConfigured)
        {
            _output.WriteLine("Integration credentials not configured; skipping.");
            return;
        }

        using var client = new AtolOnlineClient(IntegrationConfig.CreateOptions());

        var request = new ReceiptRegistrationRequest
        {
            ExternalId = $"atol-net-it-{Guid.NewGuid():N}",
            Timestamp = DateTime.Now,
            Receipt = new Receipt
            {
                Client = new Client { Email = "test@example.com" },
                Company = new Company
                {
                    Email = "test@example.com",
                    Sno = TaxationSystem.Osn,
                    Inn = IntegrationConfig.Inn ?? "5544332219",
                    PaymentAddress = IntegrationConfig.PaymentAddress ?? "https://v4.online.atol.ru",
                },
                Items =
                [
                    new Item
                    {
                        Name = "Тестовый товар",
                        Price = 10.00m,
                        Quantity = 1m,
                        Sum = 10.00m,
                        PaymentMethod = PaymentMethod.FullPayment,
                        PaymentObject = PaymentObject.Commodity,
                        Vat = new Vat { Type = VatType.None },
                    },
                ],
                Payments = [new Payment { Type = PaymentType.Cashless, Sum = 10.00m }],
                Vats = [new Vat { Type = VatType.None, Sum = 0m }],
                Total = 10.00m,
            },
        };

        var registered = await client.SellAsync(request);
        _output.WriteLine($"Registered: uuid={registered.Uuid}, status={registered.Status}, error={registered.Error?.Text}");

        Assert.NotNull(registered.Uuid);
        Assert.NotEqual(DocumentStatus.Fail, registered.Status);

        // Poll until terminal (done/fail) honoring the 1 req/sec limit. Up to ~30s.
        DocumentReportResponse report = null!;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(30))
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            report = await client.GetReportAsync(registered.Uuid!);
            _output.WriteLine($"Poll: status={report.Status}, error={report.Error?.Text}");
            if (report.Status is DocumentStatus.Done or DocumentStatus.Fail)
            {
                break;
            }
        }

        if (report.Status == DocumentStatus.Done)
        {
            Assert.NotNull(report.Payload);
            _output.WriteLine($"Fiscalized: FD={report.Payload!.FiscalDocumentNumber}, FPD={report.Payload.FiscalDocumentAttribute}, OFD={report.Payload.OfdReceiptUrl}");
        }
    }
}
