using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared
{
    public partial class BaseLayout
    {
        private ClientPreference? _themePreference;
        private MudTheme _currentTheme = new LightTheme();
        private bool _themeDrawerOpen;
        private bool _rightToLeft = false;
        private async Task RightToLeftToggle(bool value)
        {
            _rightToLeft = value;
            await Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            _themePreference = await _clientPreferenceManager.GetPreference() as ClientPreference;
            _currentTheme = _themePreference.IsDarkMode ? new DarkTheme() : new LightTheme();
            _currentTheme.Palette.Primary = _themePreference.PrimaryColor;

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

        // private async Task DarkMode()
        // {
        //     bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
        //     _currentTheme = isDarkMode
        //         ? new LightTheme()
        //         : new DarkTheme();
        //     _currentTheme.Palette.Primary = await _clientPreferenceManager.GetPrimaryColorAsync();
        // }
        private async Task ThemePreferenceChanged(ClientPreference themePreference)
        {
            _themePreference = themePreference;
            _currentTheme = _themePreference.IsDarkMode ? new DarkTheme() : new LightTheme();
            _currentTheme.Palette.Primary = _themePreference.PrimaryColor;
            await _clientPreferenceManager.SetPreference(themePreference);
        }

        public void Dispose()
        {
        }
    }
}
