using System.Net;
using AtolOnlineNet.Models;

namespace AtolOnlineNet;

/// <summary>Base type for all errors raised by the ATOL Online SDK.</summary>
public abstract class AtolOnlineException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="AtolOnlineException"/> class.</summary>
    protected AtolOnlineException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// A transport-level failure (DNS, TLS, connection reset, timeout) — no usable HTTP response was received.
/// </summary>
public sealed class AtolOnlineTransportException : AtolOnlineException
{
    /// <summary>Initializes a new instance of the <see cref="AtolOnlineTransportException"/> class.</summary>
    public AtolOnlineTransportException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// A protocol-level failure: a response was received but could not be understood (non-JSON body,
/// unexpected HTTP status with no parseable error envelope, empty token, etc.).
/// </summary>
public sealed class AtolOnlineProtocolException : AtolOnlineException
{
    /// <summary>Initializes a new instance of the <see cref="AtolOnlineProtocolException"/> class.</summary>
    public AtolOnlineProtocolException(
        string message,
        HttpStatusCode? statusCode = null,
        string? responseBodyPreview = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBodyPreview = responseBodyPreview;
    }

    /// <summary>HTTP status code of the offending response, if any.</summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>A truncated preview of the response body, useful for diagnostics.</summary>
    public string? ResponseBodyPreview { get; }
}

/// <summary>
/// Thrown when ATOL Online returns an enum value this SDK version does not recognize — almost always a
/// value ATOL added after this package was published. Please report it (with the value below) so it can
/// be added to the SDK.
/// </summary>
public sealed class AtolOnlineUnknownEnumValueException : AtolOnlineException
{
    /// <summary>URL for reporting the missing value.</summary>
    public const string IssuesUrl = "https://github.com/ai-iskuzhin/AtolOnlineNet/issues/new";

    /// <summary>Initializes a new instance of the <see cref="AtolOnlineUnknownEnumValueException"/> class.</summary>
    public AtolOnlineUnknownEnumValueException(Type enumType, string? wireValue)
        : base(BuildMessage(enumType, wireValue))
    {
        EnumType = enumType;
        WireValue = wireValue;
    }

    /// <summary>The .NET enum type that failed to map.</summary>
    public Type EnumType { get; }

    /// <summary>The unrecognized wire value received from ATOL Online.</summary>
    public string? WireValue { get; }

    private static string BuildMessage(Type enumType, string? wireValue)
        => $"ATOL Online returned value '{wireValue}', which is not a known '{enumType.Name}' in this version of AtolOnlineNet. "
           + "This is most likely a value ATOL added after this package was released — it is an SDK gap, not a problem with your code. "
           + $"Please open an issue so it can be added (include the value '{wireValue}'): {IssuesUrl}";
}

/// <summary>
/// A business error reported by ATOL Online in the response <c>error</c> envelope (for example a failed
/// authentication, an invalid token, or a validation error).
/// </summary>
public sealed class AtolOnlineApiException : AtolOnlineException
{
    /// <summary>Initializes a new instance of the <see cref="AtolOnlineApiException"/> class.</summary>
    public AtolOnlineApiException(AtolError error, HttpStatusCode? statusCode = null)
        : base(BuildMessage(error))
    {
        Error = error;
        StatusCode = statusCode;
    }

    /// <summary>The error envelope returned by the service.</summary>
    public AtolError Error { get; }

    /// <summary>Numeric error code (shortcut for <c>Error.Code</c>).</summary>
    public int Code => Error.Code;

    /// <summary>HTTP status code of the response, if available.</summary>
    public HttpStatusCode? StatusCode { get; }

    private static string BuildMessage(AtolError error)
        => $"ATOL Online error {error.Code} ({error.Type}): {error.Text} [error_id: {error.ErrorId}]";
}
