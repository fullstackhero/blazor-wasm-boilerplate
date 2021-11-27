using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Shared
{
    public partial class MainLayout
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }

        [Parameter]
        public EventCallback<bool> OnRightToLeftToggle { get; set; }
        private bool _drawerOpen = false;
        private bool _rightToLeft = false;

        private async Task RightToLeftToggle()
        {
            bool isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;

            await OnRightToLeftToggle.InvokeAsync(isRtl);
        }

        public async Task ToggleDarkMode()
        {
            await OnDarkModeToggle.InvokeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            if (await _clientPreferenceManager.GetPreference() is ClientPreference preference)
            {
                _rightToLeft = preference.IsRTL;
                _drawerOpen = preference.IsDrawerOpen;
            }
        }

        private async Task DrawerToggle()
        {
            _drawerOpen = await _clientPreferenceManager.ToggleDrawerAsync();
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                { nameof(Dialogs.Logout.ContentText), $"{_localizer["Logout Confirmation"]}"},
                { nameof(Dialogs.Logout.ButtonText), $"{_localizer["Logout"]}"},
                { nameof(Dialogs.Logout.Color), Color.Error}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            _dialogService.Show<Dialogs.Logout>(_localizer["Logout"], parameters, options);
        }
    }
}