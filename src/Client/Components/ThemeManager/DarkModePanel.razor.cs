using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager
{
    public partial class DarkModePanel
    {
        private bool IsDarkMode = false;
        protected override async Task OnInitializedAsync()
        {
            if (await _clientPreferenceManager.GetPreference() is not ClientPreference _themePreference) _themePreference = new ClientPreference();
            IsDarkMode = _themePreference.IsDarkMode;
        }
        [Parameter]
        public EventCallback<bool> OnIconClicked { get; set; }

        public async Task OnToggledChangedAsync(bool toggled)
        {
            await OnIconClicked.InvokeAsync(toggled);
        }
    }
}