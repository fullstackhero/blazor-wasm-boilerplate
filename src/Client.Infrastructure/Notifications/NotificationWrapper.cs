using FSH.WebApi.Shared.Notifications;
using MediatR;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Notifications;

public class NotificationWrapper<TNotificationMessage> : INotification
    where TNotificationMessage : INotificationMessage
{
    public NotificationWrapper(TNotificationMessage notification) => Notification = notification;

    public TNotificationMessage Notification { get; }
}