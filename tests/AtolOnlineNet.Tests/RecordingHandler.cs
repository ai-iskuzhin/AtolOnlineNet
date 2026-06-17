using System.Net;
using System.Text;

namespace AtolOnlineNet.Tests;

/// <summary>
/// A test <see cref="HttpMessageHandler"/> that records the outgoing request(s) and returns canned
/// responses in sequence. When more requests arrive than canned responses, the last response is reused.
/// </summary>
internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly (string Body, HttpStatusCode Status)[] _responses;
    private int _index;

    public RecordingHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
        : this((responseBody, statusCode))
    {
    }

    public RecordingHandler(params (string Body, HttpStatusCode Status)[] responses)
    {
        _responses = responses.Length > 0 ? responses : throw new ArgumentException("At least one response is required.", nameof(responses));
    }

    public List<HttpRequestMessage> Requests { get; } = new();

    public List<string?> Bodies { get; } = new();

    public HttpRequestMessage LastRequest => Requests[^1];

    public string? LastBody => Bodies[^1];

    public string? TokenHeader => LastRequest.Headers.TryGetValues("Token", out var values)
        ? string.Join(",", values)
        : null;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(request);
        Bodies.Add(request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken));

        var (body, status) = _responses[Math.Min(_index, _responses.Length - 1)];
        _index++;

        return new HttpResponseMessage(status)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };
    }
}
