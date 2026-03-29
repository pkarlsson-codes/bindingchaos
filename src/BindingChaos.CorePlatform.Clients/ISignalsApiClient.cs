using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Signals API.
/// </summary>
public interface ISignalsApiClient
{
    /// <summary>
    /// Gets a specific signal by its ID in the current locality.
    /// </summary>
    /// <param name="signalId">The ID of the signal to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The signal information.</returns>
    Task<SignalResponse> GetSignal(
        string signalId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all signals for the current locality.
    /// </summary>
    /// <param name="querySpec">The query specification for pagination and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of signals.</returns>
    Task<PaginatedResponse<SignalListItemResponse>> GetSignals(
        PaginationQuerySpec<SignalsQueryFilter> querySpec,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new signal in the current locality.
    /// </summary>
    /// <param name="request">The signal creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created signal.</returns>
    Task<CaptureSignalResponse> CaptureSignal(
        CaptureSignalRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Amplifies a signal by the current participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to amplify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The amplification response.</returns>
    Task<AmplifySignalResponse> AmplifySignal(
        string signalId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deamplifies a signal by a participant.
    /// </summary>
    /// <param name="signalId">The ID of the signal to deamplify.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated signal information.</returns>
    Task<DeamplifySignalResponse> DeamplifySignal(
        string signalId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets amplification trend data for a specific signal.
    /// </summary>
    /// <param name="signalId">The ID of the signal to get trend data for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The signal amplification trend data.</returns>
    Task<SignalAmplificationTrendResponse> GetSignalAmplificationTrendAsync(
        string signalId,
        CancellationToken cancellationToken);
}