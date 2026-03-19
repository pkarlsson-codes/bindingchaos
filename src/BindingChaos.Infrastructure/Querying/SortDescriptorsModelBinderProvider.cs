using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Provides a model binder for <see cref="IReadOnlyList{T}"/> of <see cref="SortDescriptor"/>.
/// </summary>
public sealed class SortDescriptorsModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.ModelType;

        if (modelType == typeof(IReadOnlyList<SortDescriptor>) ||
            modelType == typeof(List<SortDescriptor>) ||
            modelType == typeof(SortDescriptor[]))
        {
            return new SortDescriptorsModelBinder();
        }

        return null;
    }
}
