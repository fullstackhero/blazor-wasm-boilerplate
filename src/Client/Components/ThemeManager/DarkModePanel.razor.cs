using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager;

public partial class DarkModePanel
{
    private bool _isDarkMode;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is not ClientPreference themePreference) themePreference = new ClientPreference();
        _isDarkMode = themePreference.IsDarkMode;
    }

    [Parameter]
    public EventCallback<bool> OnIconClicked { get; set; }

    private async Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        await OnIconClicked.InvokeAsync(_isDarkMode);
    }
}