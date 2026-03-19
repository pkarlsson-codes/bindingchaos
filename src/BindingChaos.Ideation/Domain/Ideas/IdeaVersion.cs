using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Represents a version of an idea at a specific point in its evolution.
/// </summary>
public class IdeaVersion : Entity<IdeaVersionId>
{
    /// <summary>
    /// Initializes a new instance of the IdeaVersion class.
    /// </summary>
    /// <param name="id">The unique identifier for this version.</param>
    /// <param name="versionNumber">The version number.</param>
    /// <param name="title">The title of this version.</param>
    /// <param name="body">The body content of this version.</param>
    /// <param name="sourceAmendmentId">The source amendment ID, if applicable.</param>
    private IdeaVersion(IdeaVersionId id, int versionNumber, string title, string body, AmendmentId? sourceAmendmentId = null)
    {
        if (versionNumber < 1)
        {
            throw new ArgumentException("Version number must be at least 1", nameof(versionNumber));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Body cannot be empty", nameof(body));
        }

        Id = id;
        VersionNumber = versionNumber;
        Title = title;
        Body = body;
        CreatedAt = DateTimeOffset.UtcNow;
        SourceAmendmentId = sourceAmendmentId;
    }

    /// <summary>
    /// Gets the version number.
    /// </summary>
    public int VersionNumber { get; private set; }

    /// <summary>
    /// Gets the title of this version.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the body content of this version.
    /// </summary>
    public string Body { get; private set; } = string.Empty;

    /// <summary>
    /// Gets when this version was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the source amendment ID that created this version, if applicable.
    /// </summary>
    public AmendmentId? SourceAmendmentId { get; private set; }

    /// <summary>
    /// Creates a new idea version from a submitted draft.
    /// </summary>
    /// <param name="title">The title of the version.</param>
    /// <param name="body">The body content of the version.</param>
    /// <returns>A new idea version instance.</returns>
    public static IdeaVersion CreateFromDraft(string title, string body)
    {
        var id = IdeaVersionId.Generate();
        return new IdeaVersion(id, 1, title, body);
    }

    /// <summary>
    /// Creates a new idea version from an approved amendment.
    /// </summary>
    /// <param name="versionNumber">The version number.</param>
    /// <param name="title">The title of the version.</param>
    /// <param name="body">The body content of the version.</param>
    /// <param name="sourceAmendmentId">The source amendment ID.</param>
    /// <returns>A new idea version instance.</returns>
    public static IdeaVersion CreateFromAmendment(int versionNumber, string title, string body, AmendmentId sourceAmendmentId)
    {
        if (versionNumber < 2)
        {
            throw new ArgumentException("Version number must be at least 2 for amendment-based versions", nameof(versionNumber));
        }

        var id = IdeaVersionId.Generate();
        return new IdeaVersion(id, versionNumber, title, body, sourceAmendmentId);
    }
}