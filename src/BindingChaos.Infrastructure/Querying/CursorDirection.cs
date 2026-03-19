namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Direction for cursor-based pagination.
/// </summary>
public enum CursorDirection
{
    /// <summary>
    /// Forward pagination - get items after the cursor.
    /// </summary>
    Forward = 0,

    /// <summary>
    /// Backward pagination - get items before the cursor.
    /// </summary>
    Backward = 1,
}