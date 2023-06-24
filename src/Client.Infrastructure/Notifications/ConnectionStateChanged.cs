using FL.WebApi.Shared.Notifications;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Notifications;

public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;