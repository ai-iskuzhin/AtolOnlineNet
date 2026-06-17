using System.Net;
using System.Text.Json;
using AtolOnlineNet;
using AtolOnlineNet.V1_05;

namespace AtolOnlineNet.Tests;

public class AtolOnlineClientTests
{
    private const string TokenOk = """{"error":null,"token":"TEST_TOKEN","timestamp":"30.11.2017 17:58:53"}""";

    private static AtolOnlineClientOptions Options() => new()
    {
        Login = "v4-online-atol-ru",
        Password = "secret",
        GroupCode = "v4-online-atol-ru_4179",
        BaseAddress = new Uri("https://testonline.atol.ru/possystem/"),
        ApiVersion = "v4",
    };

    private static ReceiptRegistrationRequest SampleReceipt() => new()
    {
        ExternalId = "ext-1",
        Timestamp = new DateTime(2017, 2, 1, 13, 45, 0),
        Receipt = new Receipt
        {
            Client = new Client { Email = "kkt@kkt.ru" },
            Company = new Company { Email = "c@c.ru", Inn = "5544332219", PaymentAddress = "https://v4.online.atol.ru" },
            Items = [new Item { Name = "товар", Price = 100m, Quantity = 1m, Sum = 100m, Vat = new Vat { Type = VatType.Vat20 } }],
            Payments = [new Payment { Type = PaymentType.Cashless, Sum = 100m }],
            Total = 100m,
        },
    };

    [Fact]
    public async Task GetTokenAsync_PostsCredentialsToGetTokenEndpoint()
    {
        using var handler = new RecordingHandler(TokenOk);
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        var response = await client.GetTokenAsync();

        Assert.Equal("TEST_TOKEN", response.Token);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("https://testonline.atol.ru/possystem/v4/getToken", handler.LastRequest.RequestUri!.ToString());

        using var body = JsonDocument.Parse(handler.LastBody!);
        Assert.Equal("v4-online-atol-ru", body.RootElement.GetProperty("login").GetString());
        Assert.Equal("secret", body.RootElement.GetProperty("pass").GetString());
    }

    [Fact]
    public async Task GetTokenAsync_ThrowsApiExceptionOnError()
    {
        const string error = """{"error":{"error_id":"x","code":12,"text":"Неверный логин или пароль","type":"system"},"timestamp":"15.02.2018 13:00:31"}""";
        using var handler = new RecordingHandler(error);
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        var ex = await Assert.ThrowsAsync<AtolOnlineApiException>(() => client.GetTokenAsync());

        Assert.Equal(AtolServiceErrorCodes.WrongLoginOrPassword, ex.Code);
        Assert.Equal(AtolErrorType.System, ex.Error.Type);
    }

    [Fact]
    public async Task SellAsync_AuthenticatesThenPostsReceiptWithTokenHeader()
    {
        const string registered = """{"uuid":"2ea26f17-0884-4f08-b120-306fc096a58f","timestamp":"12.04.2017 06:15:06","error":null,"status":"wait"}""";
        using var handler = new RecordingHandler((TokenOk, HttpStatusCode.OK), (registered, HttpStatusCode.OK));
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        var response = await client.SellAsync(SampleReceipt());

        // First call = getToken, second = sell.
        Assert.Equal(2, handler.Requests.Count);
        Assert.Equal("2ea26f17-0884-4f08-b120-306fc096a58f", response.Uuid);
        Assert.Equal(DocumentStatus.Wait, response.Status);

        Assert.Equal("https://testonline.atol.ru/possystem/v4/v4-online-atol-ru_4179/sell", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("TEST_TOKEN", handler.TokenHeader);

        using var body = JsonDocument.Parse(handler.LastBody!);
        Assert.Equal("ext-1", body.RootElement.GetProperty("external_id").GetString());
    }

    [Fact]
    public async Task GetReportAsync_UsesGetWithUuidPath()
    {
        const string report = """{"uuid":"abc","timestamp":"12.04.2017 20:15:08","status":"done","error":null}""";
        using var handler = new RecordingHandler((TokenOk, HttpStatusCode.OK), (report, HttpStatusCode.OK));
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        var response = await client.GetReportAsync("abc");

        Assert.Equal(DocumentStatus.Done, response.Status);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Equal("https://testonline.atol.ru/possystem/v4/v4-online-atol-ru_4179/report/abc", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task CachedToken_IsReusedAcrossCalls()
    {
        const string registered = """{"uuid":"u1","timestamp":"12.04.2017 06:15:06","error":null,"status":"wait"}""";
        using var handler = new RecordingHandler((TokenOk, HttpStatusCode.OK), (registered, HttpStatusCode.OK));
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        await client.SellAsync(SampleReceipt());
        await client.SellAsync(SampleReceipt());

        // getToken once + two sells = 3 requests; token fetched only once.
        Assert.Equal(3, handler.Requests.Count);
        Assert.Single(handler.Requests, r => r.RequestUri!.AbsolutePath.EndsWith("getToken", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ExpiredToken_TriggersReauthAndRetry()
    {
        const string expired = """{"timestamp":"12.04.2017 06:15:06","status":"fail","error":{"error_id":"x","code":11,"text":"Переданный токен не активен","type":"system"}}""";
        const string registered = """{"uuid":"u2","timestamp":"12.04.2017 06:15:06","error":null,"status":"wait"}""";
        using var handler = new RecordingHandler(
            (TokenOk, HttpStatusCode.OK),        // initial getToken
            (expired, HttpStatusCode.OK),        // sell -> ExpiredToken
            (TokenOk, HttpStatusCode.OK),        // re-auth
            (registered, HttpStatusCode.OK));    // sell retry -> ok
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        var response = await client.SellAsync(SampleReceipt());

        Assert.Equal("u2", response.Uuid);
        Assert.Equal(4, handler.Requests.Count);
    }

    [Fact]
    public async Task RegisterWithoutGroupCode_Throws()
    {
        var options = new AtolOnlineClientOptions
        {
            Login = "l",
            Password = "p",
            BaseAddress = new Uri("https://testonline.atol.ru/possystem/"),
        };
        using var handler = new RecordingHandler(TokenOk);
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, options);

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.SellAsync(SampleReceipt()));
    }

    [Fact]
    public async Task DefaultUserAgent_IsSentWhenCallerHasNotConfiguredOne()
    {
        using var handler = new RecordingHandler(TokenOk);
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        await client.GetTokenAsync();

        var userAgent = handler.LastRequest.Headers.UserAgent.ToString();
        Assert.StartsWith("AtolOnlineNet/", userAgent);
    }

    [Fact]
    public async Task CallerConfiguredUserAgent_IsPreserved()
    {
        using var handler = new RecordingHandler(TokenOk);
        using var http = new HttpClient(handler);
        http.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/9.9");
        using var client = new AtolOnlineClient(http, Options());

        await client.GetTokenAsync();

        // HttpClient merges its default UA onto the request; ours must not be added on top of it.
        var userAgent = handler.LastRequest.Headers.UserAgent.ToString();
        Assert.Equal("MyApp/9.9", userAgent);
        Assert.DoesNotContain("AtolOnlineNet", userAgent);
    }

    [Fact]
    public async Task NonJsonResponse_ThrowsProtocolException()
    {
        using var handler = new RecordingHandler(("<html>502 Bad Gateway</html>", HttpStatusCode.BadGateway));
        using var http = new HttpClient(handler);
        using var client = new AtolOnlineClient(http, Options());

        await Assert.ThrowsAsync<AtolOnlineProtocolException>(() => client.GetTokenAsync());
    }
}
