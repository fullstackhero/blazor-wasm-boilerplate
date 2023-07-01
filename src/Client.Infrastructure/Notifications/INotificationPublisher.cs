using FL.WebApi.Shared.Notifications;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}