using FSH.BlazorWebAssembly.Client.Theme;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared
{
    public partial class BaseLayout
    {
        private MudTheme _currentTheme = new LightTheme();
        private bool _rightToLeft = false;
        private async Task RightToLeftToggle(bool value)
        {
            _rightToLeft = value;
            await Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = new LightTheme();
            _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            _rightToLeft = await _clientPreferenceManager.IsRTL();
        }

        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _currentTheme = isDarkMode
                ? new LightTheme()
                : new DarkTheme();
        }

        public void Dispose()
        {
        }
    }
}
