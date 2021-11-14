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
            if (_themePreference == null) _themePreference = new ClientPreference();
            SetCurrentTheme(_themePreference);
            _snackBar.Add("Like this boilerplate? ", Severity.Normal, config =>
            {
                config.BackgroundBlurred = true;
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
        private async Task ThemePreferenceChanged(ClientPreference themePreference)
        {
            SetCurrentTheme(themePreference);
            await _clientPreferenceManager.SetPreference(themePreference);
        }

        private void SetCurrentTheme(ClientPreference themePreference)
        {
            _currentTheme = themePreference.IsDarkMode ? new DarkTheme() : new LightTheme();
            _currentTheme.Palette.Primary = themePreference.PrimaryColor;
            _currentTheme.Palette.Secondary = themePreference.SecondaryColor;
            _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
            _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
        }

        public void Dispose()
        {
        }
    }
}