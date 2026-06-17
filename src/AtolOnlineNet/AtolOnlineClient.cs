using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AtolOnlineNet.Models;
using AtolOnlineNet.Serialization;

namespace AtolOnlineNet;

/// <summary>
/// Default implementation of <see cref="IAtolOnlineClient"/>. Handles token acquisition and caching,
/// JSON (de)serialization, and a single transparent retry when the cached token has expired.
/// </summary>
public sealed class AtolOnlineClient : IAtolOnlineClient, IDisposable
{
    private const string TokenHeader = "Token";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);
    private static readonly TimeSpan TokenRefreshBuffer = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Default <c>User-Agent</c> applied per-request when the caller has not set one
    /// (for example <c>AtolOnlineNet/0.1.0</c>).
    /// </summary>
    private static readonly ProductInfoHeaderValue[] DefaultUserAgent = BuildUserAgent();

    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly AtolOnlineClientOptions _options;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private string? _token;
    private DateTimeOffset _tokenExpiresAt;

    /// <summary>Initializes a new client over a caller-supplied <see cref="HttpClient"/> (recommended).</summary>
    public AtolOnlineClient(HttpClient httpClient, AtolOnlineClientOptions options)
        : this(httpClient, options, ownsHttpClient: false)
    {
    }

    /// <summary>Initializes a new client that creates and owns its own <see cref="HttpClient"/>.</summary>
    public AtolOnlineClient(AtolOnlineClientOptions options)
        : this(new HttpClient(), options, ownsHttpClient: true)
    {
    }

    private AtolOnlineClient(HttpClient httpClient, AtolOnlineClientOptions options, bool ownsHttpClient)
    {
        _httpClient = Guard.NotNull(httpClient);
        _options = Guard.NotNull(options);
        Guard.NotNullOrWhiteSpace(options.Login, $"{nameof(options)}.{nameof(options.Login)}");
        Guard.NotNullOrWhiteSpace(options.Password, $"{nameof(options)}.{nameof(options.Password)}");
        Guard.NotNullOrWhiteSpace(options.ApiVersion, $"{nameof(options)}.{nameof(options.ApiVersion)}");
        Guard.NotNull(options.BaseAddress, $"{nameof(options)}.{nameof(options.BaseAddress)}");
        _ownsHttpClient = ownsHttpClient;
    }

    /// <inheritdoc />
    public async Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var body = new TokenRequestBody
        {
            Login = _options.Login,
            Pass = _options.Password,
            Source = _options.Source,
        };

        var response = await SendAsync<TokenResponse>(
            HttpMethod.Post,
            BuildUri("getToken"),
            body,
            token: null,
            cancellationToken).ConfigureAwait(false);

        if (response.Error is { } error)
        {
            throw new AtolOnlineApiException(error);
        }

        if (string.IsNullOrEmpty(response.Token))
        {
            throw new AtolOnlineProtocolException("Authorization succeeded but the response contained no token.");
        }

        _token = response.Token;
        _tokenExpiresAt = DateTimeOffset.UtcNow + TokenLifetime - TokenRefreshBuffer;
        return response;
    }

    /// <inheritdoc />
    public Task<RegisterDocumentResponse> RegisterAsync(
        ReceiptOperation operation,
        object request,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(request);
        return RegisterDocumentAsync(operation.ToWire(), request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<RegisterDocumentResponse> RegisterCorrectionAsync(
        CorrectionOperation operation,
        object request,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(request);
        return RegisterDocumentAsync(operation.ToWire(), request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DocumentReportResponse> GetReportAsync(string uuid, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(uuid);
        var groupCode = RequireGroupCode();
        var uri = BuildUri($"{groupCode}/report/{Uri.EscapeDataString(uuid)}");

        return await SendWithTokenAsync<DocumentReportResponse>(
            HttpMethod.Get,
            uri,
            body: null,
            r => r.Error,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _tokenLock.Dispose();
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private Task<RegisterDocumentResponse> RegisterDocumentAsync(
        string operation,
        object request,
        CancellationToken cancellationToken)
    {
        var groupCode = RequireGroupCode();
        var uri = BuildUri($"{groupCode}/{operation}");
        return SendWithTokenAsync<RegisterDocumentResponse>(
            HttpMethod.Post,
            uri,
            request,
            r => r.Error,
            cancellationToken);
    }

    /// <summary>
    /// Sends a token-authenticated request, transparently refreshing the token and retrying once if the
    /// service reports an expired token in the response envelope.
    /// </summary>
    private async Task<TResponse> SendWithTokenAsync<TResponse>(
        HttpMethod method,
        Uri uri,
        object? body,
        Func<TResponse, AtolError?> errorSelector,
        CancellationToken cancellationToken)
    {
        var token = await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var response = await SendAsync<TResponse>(method, uri, body, token, cancellationToken).ConfigureAwait(false);

        if (errorSelector(response)?.Code == AtolServiceErrorCodes.ExpiredToken)
        {
            token = await RefreshTokenAsync(force: true, cancellationToken).ConfigureAwait(false);
            response = await SendAsync<TResponse>(method, uri, body, token, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private async Task<string> EnsureTokenAsync(CancellationToken cancellationToken)
    {
        if (_token is not null && DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            return _token;
        }

        return await RefreshTokenAsync(force: false, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> RefreshTokenAsync(bool force, CancellationToken cancellationToken)
    {
        var tokenBeforeWait = _token;

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // When forcing, only re-authenticate if nobody else already replaced the token we saw expire.
            // Otherwise reuse a still-valid cached token that another caller may have refreshed meanwhile.
            var stillValid = _token is not null && DateTimeOffset.UtcNow < _tokenExpiresAt;
            if (force ? !ReferenceEquals(_token, tokenBeforeWait) && stillValid : stillValid)
            {
                return _token!;
            }

            await GetTokenAsync(cancellationToken).ConfigureAwait(false);
            return _token!;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private async Task<TResponse> SendAsync<TResponse>(
        HttpMethod method,
        Uri uri,
        object? body,
        string? token,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (token is not null)
        {
            request.Headers.TryAddWithoutValidation(TokenHeader, token);
        }

        // Only set our User-Agent when the caller's HttpClient has not configured one of its own.
        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0 && request.Headers.UserAgent.Count == 0)
        {
            foreach (var product in DefaultUserAgent)
            {
                request.Headers.UserAgent.Add(product);
            }
        }

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, body.GetType(), AtolJson.Options);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new AtolOnlineTransportException($"HTTP request to '{uri}' failed: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new AtolOnlineTransportException($"HTTP request to '{uri}' timed out.", ex);
        }

        using (httpResponse)
        {
#if NETSTANDARD2_0
            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new AtolOnlineProtocolException(
                    $"Empty response body (HTTP {(int)httpResponse.StatusCode}).",
                    httpResponse.StatusCode);
            }

            try
            {
                var result = JsonSerializer.Deserialize<TResponse>(content, AtolJson.Options);
                if (result is null)
                {
                    throw new AtolOnlineProtocolException(
                        $"Response deserialized to null (HTTP {(int)httpResponse.StatusCode}).",
                        httpResponse.StatusCode,
                        Preview(content));
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new AtolOnlineProtocolException(
                    $"Failed to parse ATOL Online response (HTTP {(int)httpResponse.StatusCode}): {ex.Message}",
                    httpResponse.StatusCode,
                    Preview(content),
                    ex);
            }
        }
    }

    private Uri BuildUri(string relativePath) => new(_options.BaseAddress, $"{_options.ApiVersion}/{relativePath}");

    private string RequireGroupCode()
        => _options.GroupCode is { Length: > 0 } code
            ? code
            : throw new InvalidOperationException(
                $"{nameof(AtolOnlineClientOptions)}.{nameof(AtolOnlineClientOptions.GroupCode)} must be set to register documents or fetch results.");

    private static string Preview(string content) => content.Length <= 512 ? content : content.Substring(0, 512);

    private static ProductInfoHeaderValue[] BuildUserAgent()
    {
        var assembly = typeof(AtolOnlineClient).Assembly;
        var name = assembly.GetName().Name ?? "AtolOnlineNet";

        // Prefer the informational version (e.g. "0.1.0-preview.1+<sha>"); strip SourceLink build metadata.
        var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var version = informational?.Split('+')[0];
        if (string.IsNullOrWhiteSpace(version))
        {
            version = assembly.GetName().Version?.ToString() ?? "0.0.0";
        }

        return [new ProductInfoHeaderValue(name, version)];
    }

    private sealed record TokenRequestBody
    {
        public required string Login { get; init; }

        public required string Pass { get; init; }

        public string? Source { get; init; }
    }
}
