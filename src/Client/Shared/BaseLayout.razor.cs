using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;
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

            _snackBar.Add("Like this boilerplate? ", Severity.Normal, config =>
            {
                config.BackgroundBlurred = true;
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
                config.Icon = Icons.Custom.Brands.GitHub;
                config.Action = "Star us on Github!";
                config.ActionColor = Color.Primary;
                config.Onclick = snackbar =>
                {
                    _navigationManager.NavigateTo("https://github.com/fullstackhero/blazor-wasm-boilerplate");
                    return Task.CompletedTask;
                };
            });
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
