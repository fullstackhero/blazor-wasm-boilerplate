using FL.WebApi.Shared.Notifications;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Preferences;

public class FLTablePreference : INotificationMessage
{
    public bool IsDense { get; set; }
    public bool IsStriped { get; set; }
    public bool HasBorder { get; set; }
    public bool IsHoverable { get; set; }
}