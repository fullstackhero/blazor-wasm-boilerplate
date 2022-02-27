using FSH.WebApi.Shared.Notifications;
using MediatR;
using MediatR.Courier;
using MediatR.Courier.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;

internal static class Startup
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        // Add mediator processing of notifications
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services
            .AddMediatR(assemblies)
            .AddCourier(assemblies)
            .AddTransient<INotificationPublisher, NotificationPublisher>();

        // Register handlers for all INotificationMessages
        foreach (var eventType in assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i => i == typeof(INotificationMessage))))
        {
            services.AddSingleton(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(NotificationWrapper<>).MakeGenericType(eventType)),
                serviceProvider => serviceProvider.GetRequiredService(typeof(MediatRCourier)));
        }

        return services;
    }
}