using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager
{
    public partial class ThemeDrawer
    {

        [EditorRequired] [Parameter] public bool ThemeDrawerOpen { get; set; }
        [EditorRequired] [Parameter] public EventCallback<bool> ThemeDrawerOpenChanged { get; set; }
        [Parameter] public EventCallback OnDarkModeToggle { get; set; }
    }
}
