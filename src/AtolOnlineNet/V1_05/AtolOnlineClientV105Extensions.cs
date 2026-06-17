using System.Threading;
using System.Threading.Tasks;
using AtolOnlineNet.Models;

namespace AtolOnlineNet.V1_05;

/// <summary>
/// Strongly-typed FFD 1.05 (service <c>v4</c>) convenience helpers over <see cref="IAtolOnlineClient"/>.
/// Each method maps to a registration operation with the FFD 1.05 receipt/correction request models.
/// </summary>
public static class AtolOnlineClientV105Extensions
{
    /// <summary>Registers a «Приход» (<c>sell</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> SellAsync(
        this IAtolOnlineClient client, ReceiptRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterAsync(ReceiptOperation.Sell, request, cancellationToken);

    /// <summary>Registers a «Возврат прихода» (<c>sell_refund</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> SellRefundAsync(
        this IAtolOnlineClient client, ReceiptRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterAsync(ReceiptOperation.SellRefund, request, cancellationToken);

    /// <summary>Registers a «Расход» (<c>buy</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> BuyAsync(
        this IAtolOnlineClient client, ReceiptRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterAsync(ReceiptOperation.Buy, request, cancellationToken);

    /// <summary>Registers a «Возврат расхода» (<c>buy_refund</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> BuyRefundAsync(
        this IAtolOnlineClient client, ReceiptRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterAsync(ReceiptOperation.BuyRefund, request, cancellationToken);

    /// <summary>Registers a «Коррекция прихода» (<c>sell_correction</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> SellCorrectionAsync(
        this IAtolOnlineClient client, CorrectionRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterCorrectionAsync(CorrectionOperation.SellCorrection, request, cancellationToken);

    /// <summary>Registers a «Коррекция расхода» (<c>buy_correction</c>) receipt.</summary>
    public static Task<RegisterDocumentResponse> BuyCorrectionAsync(
        this IAtolOnlineClient client, CorrectionRegistrationRequest request, CancellationToken cancellationToken = default)
        => Guard.NotNull(client).RegisterCorrectionAsync(CorrectionOperation.BuyCorrection, request, cancellationToken);
}
