using System.Text.RegularExpressions;
using Blazored.LocalStorage;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;
using FSH.BlazorWebAssembly.Shared.Preference;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Managers.Preferences
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> ToggleDarkModeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDarkMode = !preference.IsDarkMode;
                await SetPreference(preference);
                return !preference.IsDarkMode;
            }

            return false;
        }

        public async Task<bool> ToggleDrawerAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDrawerOpen = !preference.IsDrawerOpen;
                await SetPreference(preference);
                return preference.IsDrawerOpen;
            }

            return false;
        }

        public async Task<bool> ToggleLayoutDirection()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsRTL = !preference.IsRTL;
                await SetPreference(preference);
                return preference.IsRTL;
            }
            return false;
        }

        public async Task<IResult> ChangeLanguageAsync(string languageCode)
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                // preference.LanguageCode = languageCode;
                await SetPreference(preference);
                return new Result
                {
                    Succeeded = true,
                    Messages = new List<string> { "Client Language has been changed" }
                };
            }

            return new Result
            {
                Succeeded = false,
                Messages = new List<string> { "Failed to get client preferences" }
            };
        }

        public async Task<MudTheme> GetCurrentThemeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                if (preference.IsDarkMode == true) return new DarkTheme();
            }
            return new LightTheme();
        }

        public async Task<string> GetPrimaryColorAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                var colorCode = preference.PrimaryColor;
                if (Regex.Match(colorCode, "^#(?:[0-9a-fA-F]{3,4}){1,2}$").Success)
                    return colorCode;
                else
                {
                    preference.PrimaryColor = CustomColors.Light.Primary;
                    await SetPreference(preference);
                    return preference.PrimaryColor;
                }
            }
            return CustomColors.Light.Primary;
        }

        public async Task<bool> IsRTL()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                return preference.IsRTL;
            }
            return false;
        }

        public async Task<bool> IsDrawerOpen()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                return preference.IsDrawerOpen;
            }
            return false;
        }

        public static string Preference = "clientPreference";

        public async Task<IPreference> GetPreference()
        {
            return await _localStorageService.GetItemAsync<ClientPreference>(Preference) ?? new ClientPreference();
        }

        public async Task SetPreference(IPreference preference)
        {
            await _localStorageService.SetItemAsync(Preference, preference as ClientPreference);
        }
    }
}