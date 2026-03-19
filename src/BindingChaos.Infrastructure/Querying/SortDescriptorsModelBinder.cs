using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Binds a comma-separated sort string into a list of SortDescriptor.
/// Supported tokens: "-field", "field", and "field:asc|desc".
/// </summary>
public sealed class SortDescriptorsModelBinder : IModelBinder
{
    /// <summary>
    /// Binds a comma-separated sort string into a list of <see cref="SortDescriptor"/>.
    /// </summary>
    /// <param name="bindingContext">The model binding context.</param>
    /// <returns>A task that completes when binding is done.</returns>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Success(Array.Empty<SortDescriptor>());
            return Task.CompletedTask;
        }

        var sortRaw = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(sortRaw))
        {
            bindingContext.Result = ModelBindingResult.Success(Array.Empty<SortDescriptor>());
            return Task.CompletedTask;
        }

        List<SortDescriptor> descriptors = [];
        var tokens = sortRaw.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in tokens)
        {
            var token = raw.Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                continue;
            }

            SortDirection direction = SortDirection.Asc;
            string field = token;

            if (token.Length > 0 && token[0] == '-')
            {
                direction = SortDirection.Desc;
                field = token.Substring(1).Trim();
            }
            else
            {
                var colonIndex = token.IndexOf(':');
                if (colonIndex > 0)
                {
                    field = token.Substring(0, colonIndex).Trim();
                    var dir = token.Substring(colonIndex + 1).Trim();
                    if (dir.Equals("desc", StringComparison.OrdinalIgnoreCase) || dir.Equals("descending", StringComparison.OrdinalIgnoreCase))
                    {
                        direction = SortDirection.Desc;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(field))
            {
                descriptors.Add(new SortDescriptor(field, direction));
            }
        }

        bindingContext.Result = ModelBindingResult.Success(descriptors.ToArray());
        return Task.CompletedTask;
    }
}
