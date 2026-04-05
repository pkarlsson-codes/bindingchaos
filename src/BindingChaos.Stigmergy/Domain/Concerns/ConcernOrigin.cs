namespace BindingChaos.Stigmergy.Domain.Concerns;

/// <summary>
/// Describes how a concern came to be raised.
/// </summary>
public enum ConcernOrigin
{
    /// <summary>
    /// A participant manually identified a pattern and raised the concern directly.
    /// </summary>
    Manual = 1,

    /// <summary>
    /// A signal cluster was detected and a participant confirmed it as a concern.
    /// </summary>
    EmergingPattern = 2,
}
