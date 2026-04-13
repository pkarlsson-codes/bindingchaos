namespace BindingChaos.Stigmergy.Application.DTOs;

/// <summary>Lifecycle status of a project, used in read models and queries.</summary>
public enum ProjectStatusDto
{
    /// <summary>The project is active and accepting amendments.</summary>
    Active,

    /// <summary>The project has been completed.</summary>
    Completed,

    /// <summary>The project has been archived.</summary>
    Archived,
}
