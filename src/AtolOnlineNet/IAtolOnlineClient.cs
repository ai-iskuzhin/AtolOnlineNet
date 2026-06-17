using System.Threading;
using System.Threading.Tasks;
using AtolOnlineNet.Models;

namespace AtolOnlineNet;

/// <summary>
/// Client for the ATOL Online cloud fiscalization API (<c>possystem</c>): authenticate, register
/// documents, and poll fiscalization results.
/// <para>
/// The client is FFD-version-neutral — the transport (token / register / report) is identical across
/// service versions. Strongly-typed receipt helpers live in the version-specific namespaces
/// (for example <c>AtolOnlineNet.V1_05</c> for ФФД 1.05); the FFD version is selected via
/// <see cref="AtolOnlineClientOptions.ApiVersion"/>.
/// </para>
/// </summary>
public interface IAtolOnlineClient
{
    /// <summary>
    /// Requests a fresh authorization token (<c>getToken</c>) and caches it for subsequent calls.
    /// Throws <see cref="AtolOnlineApiException"/> if authentication fails.
    /// </summary>
    Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a sell/buy/refund document. <paramref name="request"/> is the FFD-version-specific
    /// receipt request object (for example <c>AtolOnlineNet.V1_05.ReceiptRegistrationRequest</c>),
    /// serialized as the POST body.
    /// </summary>
    Task<RegisterDocumentResponse> RegisterAsync(
        ReceiptOperation operation,
        object request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a correction document. <paramref name="request"/> is the FFD-version-specific
    /// correction request object (for example <c>AtolOnlineNet.V1_05.CorrectionRegistrationRequest</c>).
    /// </summary>
    Task<RegisterDocumentResponse> RegisterCorrectionAsync(
        CorrectionOperation operation,
        object request,
        CancellationToken cancellationToken = default);

    /// <summary>Retrieves the processing result for a previously registered document by its UUID.</summary>
    Task<DocumentReportResponse> GetReportAsync(string uuid, CancellationToken cancellationToken = default);
}
