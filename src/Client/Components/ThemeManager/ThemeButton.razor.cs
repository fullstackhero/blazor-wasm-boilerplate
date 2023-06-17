using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FL_CRMS_ERP_WASM.Client.Components.ThemeManager;

public partial class ThemeButton
{
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}