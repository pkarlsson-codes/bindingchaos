using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>Per-inquiry read model projected from <see cref="Domain.ProjectInquiries.ProjectInquiry"/> events.</summary>
public class ProjectInquiryView
{
    /// <summary>Maps sortable property names to expressions for ordering query results.</summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<ProjectInquiryView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<ProjectInquiryView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["raisedAt"] = x => x.RaisedAt,
            ["status"] = x => x.Status,
        };

    /// <summary>The inquiry identifier (stream id).</summary>
    required public string Id { get; set; }

    /// <summary>The project this inquiry is about.</summary>
    required public string ProjectId { get; set; }

    /// <summary>The participant who raised the inquiry.</summary>
    required public string RaisedByParticipantId { get; set; }

    /// <summary>The society that gives the raiser standing.</summary>
    required public string RaisedBySocietyId { get; set; }

    /// <summary>The inquiry body text.</summary>
    required public string Body { get; set; }

    /// <summary>The current lifecycle status string.</summary>
    required public string Status { get; set; }

    /// <summary>The user group's response, when provided.</summary>
    public string? Response { get; set; }

    /// <summary>The linked discourse thread, when created.</summary>
    public string? DiscourseThreadId { get; set; }

    /// <summary>When the inquiry was raised.</summary>
    required public DateTimeOffset RaisedAt { get; set; }

    /// <summary>When the inquiry was last acted on.</summary>
    required public DateTimeOffset LastUpdatedAt { get; set; }
}
