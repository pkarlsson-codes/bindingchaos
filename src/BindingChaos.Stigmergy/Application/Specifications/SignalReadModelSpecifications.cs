using System.Linq.Expressions;
using BindingChaos.SharedKernel.Specifications;
using BindingChaos.Stigmergy.Application.ReadModels;

namespace BindingChaos.Stigmergy.Application.Specifications;

/// <summary>
/// Matches signals captured at or after a cutoff timestamp.
/// </summary>
internal sealed class SignalsCapturedSinceSpecification : Specification<SignalsListItemView>
{
    private readonly DateTimeOffset _cutoffDate;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsCapturedSinceSpecification"/> class.
    /// </summary>
    /// <param name="cutoffDate">The capture timestamp lower bound.</param>
    public SignalsCapturedSinceSpecification(DateTimeOffset cutoffDate)
    {
        _cutoffDate = cutoffDate;
    }

    /// <summary>
    /// Creates an optional captured-since specification.
    /// </summary>
    /// <param name="cutoffDate">The optional cutoff timestamp.</param>
    /// <returns>A captured-since specification when provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional(DateTimeOffset? cutoffDate)
    {
        return cutoffDate.HasValue
            ? new SignalsCapturedSinceSpecification(cutoffDate.Value)
            : Specification<SignalsListItemView>.All;
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal => signal.CapturedAt >= _cutoffDate;
    }
}

/// <summary>
/// Matches signals whose title, description, or tags contain a search term.
/// </summary>
internal sealed class SignalsMatchingSearchTermSpecification : Specification<SignalsListItemView>
{
    private readonly string _searchTerm;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsMatchingSearchTermSpecification"/> class.
    /// </summary>
    /// <param name="searchTerm">The search term to match.</param>
    public SignalsMatchingSearchTermSpecification(string searchTerm)
    {
        _searchTerm = !string.IsNullOrWhiteSpace(searchTerm)
            ? searchTerm
            : throw new ArgumentException("Search term cannot be null or whitespace.", nameof(searchTerm));
    }

    /// <summary>
    /// Creates an optional search-term specification.
    /// </summary>
    /// <param name="searchTerm">The optional search term.</param>
    /// <returns>A search-term specification when provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional(string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm)
            ? Specification<SignalsListItemView>.All
            : new SignalsMatchingSearchTermSpecification(searchTerm);
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal =>
            signal.Title.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
            signal.Description.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
            signal.Tags.Any(tag => tag.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Matches signals that contain at least one requested tag.
/// </summary>
internal sealed class SignalsWithAnyTagsSpecification : Specification<SignalsListItemView>
{
    private readonly string[] _tags;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsWithAnyTagsSpecification"/> class.
    /// </summary>
    /// <param name="tags">The tags to match.</param>
    public SignalsWithAnyTagsSpecification(string[] tags)
    {
        _tags = tags ?? throw new ArgumentNullException(nameof(tags));
    }

    /// <summary>
    /// Creates an optional tag filter specification.
    /// </summary>
    /// <param name="tags">The optional tag list.</param>
    /// <returns>A tag-filter specification when valid tags are provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional(IEnumerable<string>? tags)
    {
        var validTags = (tags ?? [])
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToArray();

        return validTags.Length == 0
            ? Specification<SignalsListItemView>.All
            : new SignalsWithAnyTagsSpecification(validTags);
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal => signal.Tags.Any(tag => _tags.Contains(tag));
    }
}

/// <summary>
/// Matches signals amplified by a specific participant.
/// </summary>
internal sealed class SignalsAmplifiedByParticipantSpecification : Specification<SignalsListItemView>
{
    private readonly string _participantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsAmplifiedByParticipantSpecification"/> class.
    /// </summary>
    /// <param name="participantId">The participant ID to filter by.</param>
    public SignalsAmplifiedByParticipantSpecification(string participantId)
    {
        _participantId = !string.IsNullOrWhiteSpace(participantId)
            ? participantId
            : throw new ArgumentException("Participant ID cannot be null or whitespace.", nameof(participantId));
    }

    /// <summary>
    /// Creates an optional amplified-by specification.
    /// </summary>
    /// <param name="participantId">The optional participant ID.</param>
    /// <returns>An amplified-by specification when provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional(string? participantId)
    {
        return string.IsNullOrWhiteSpace(participantId)
            ? Specification<SignalsListItemView>.All
            : new SignalsAmplifiedByParticipantSpecification(participantId);
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal => signal.AmplifierIds.Contains(_participantId);
    }
}

/// <summary>
/// Matches signals captured by a specific participant.
/// </summary>
internal sealed class SignalsCapturedByParticipantSpecification : Specification<SignalsListItemView>
{
    private readonly string _participantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsCapturedByParticipantSpecification"/> class.
    /// </summary>
    /// <param name="participantId">The participant ID to filter by.</param>
    public SignalsCapturedByParticipantSpecification(string participantId)
    {
        _participantId = !string.IsNullOrWhiteSpace(participantId)
            ? participantId
            : throw new ArgumentException("Participant ID cannot be null or whitespace.", nameof(participantId));
    }

    /// <summary>
    /// Creates an optional captured-by specification.
    /// </summary>
    /// <param name="participantId">The optional participant ID.</param>
    /// <returns>A captured-by specification when provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional(string? participantId)
    {
        return string.IsNullOrWhiteSpace(participantId)
            ? Specification<SignalsListItemView>.All
            : new SignalsCapturedByParticipantSpecification(participantId);
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal => signal.CapturedById == _participantId;
    }
}

/// <summary>
/// Matches signals whose amplification count falls within a range.
/// </summary>
internal sealed class SignalsByAmplificationRangeSpecification : Specification<SignalsListItemView>
{
    private readonly int _min;
    private readonly int _max;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalsByAmplificationRangeSpecification"/> class.
    /// </summary>
    /// <param name="min">Minimum amplification count.</param>
    /// <param name="max">Maximum amplification count.</param>
    public SignalsByAmplificationRangeSpecification(int min, int max)
    {
        _min = min;
        _max = max;
    }

    /// <summary>
    /// Creates an optional amplification-range specification.
    /// </summary>
    /// <param name="range">The optional amplification range.</param>
    /// <returns>A range specification when provided; otherwise, an identity specification.</returns>
    public static Specification<SignalsListItemView> Optional((int min, int max)? range)
    {
        return range.HasValue
            ? new SignalsByAmplificationRangeSpecification(range.Value.min, range.Value.max)
            : Specification<SignalsListItemView>.All;
    }

    /// <inheritdoc />
    public override Expression<Func<SignalsListItemView, bool>> ToExpression()
    {
        return signal => signal.AmplificationCount >= _min && signal.AmplificationCount <= _max;
    }
}
