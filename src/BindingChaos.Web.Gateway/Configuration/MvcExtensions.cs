using System.Text.Json.Serialization;
using BindingChaos.Infrastructure.Querying;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// MVC registration helpers.
/// </summary>
public static class MvcExtensions
{
    /// <summary>
    /// Adds controllers and registers the custom sort descriptors model binder.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMvcWithQuerying(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new SortDescriptorsModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
