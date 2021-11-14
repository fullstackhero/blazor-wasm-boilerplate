using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager
{
    public partial class ThemeDrawer
    {

        [EditorRequired] [Parameter] public bool ThemeDrawerOpen { get; set; }
        [EditorRequired] [Parameter] public EventCallback<bool> ThemeDrawerOpenChanged { get; set; }
        [EditorRequired] [Parameter] public ClientPreference? ThemePreference { get; set; }
        [EditorRequired] [Parameter] public EventCallback<ClientPreference> ThemePreferenceChanged { get; set; }

        private readonly List<string> _primaryColors = new()
        {
            Colors.Green.Default,
            Colors.Blue.Default,
            Colors.BlueGrey.Default,
            Colors.Purple.Default,
            Colors.Orange.Default,
            Colors.Red.Default
        };

        private async Task UpdateThemePrimaryColor(string color)
        {
            if (ThemePreference is not null)
            {
                ThemePreference.PrimaryColor = color;
                await ThemePreferenceChanged.InvokeAsync(ThemePreference);
            }
        }
        private async Task ToggleDarkLightMode(bool isDarkMode)
        {
            if (ThemePreference is not null)
            {
                ThemePreference.IsDarkMode = isDarkMode;
                await ThemePreferenceChanged.InvokeAsync(ThemePreference);
            }
        }
    }
}
