using FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared;

public partial class MainLayout
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public EventCallback OnDarkModeToggle { get; set; }

    [Parameter]
    public EventCallback<bool> OnRightToLeftToggle { get; set; }

    [Inject]
    public NotificationClient Notifications { get; set; } = default!;

    private bool _drawerOpen;
    private bool _rightToLeft;

    protected override async Task OnInitializedAsync()
    {
        // launch the signalR connection in the background.
        // see https://www.dotnetcurry.com/aspnet-core/realtime-app-using-blazor-webassembly-signalr-csharp9
#pragma warning disable CS4014
        Notifications.TryConnectAsync();
#pragma warning restore CS4014

        if (await ClientPreferences.GetPreference() is ClientPreference preference)
        {
            _rightToLeft = preference.IsRTL;
            _drawerOpen = preference.IsDrawerOpen;
        }
    }

    private async Task RightToLeftToggle()
    {
        bool isRtl = await ClientPreferences.ToggleLayoutDirectionAsync();
        _rightToLeft = isRtl;

        await OnRightToLeftToggle.InvokeAsync(isRtl);
    }

    public async Task ToggleDarkMode()
    {
        await OnDarkModeToggle.InvokeAsync();
    }

    private async Task DrawerToggle()
    {
        _drawerOpen = await ClientPreferences.ToggleDrawerAsync();
    }

    private void Logout()
    {
        var parameters = new DialogParameters
            {
                { nameof(Dialogs.Logout.ContentText), $"{L["Logout Confirmation"]}"},
                { nameof(Dialogs.Logout.ButtonText), $"{L["Logout"]}"},
                { nameof(Dialogs.Logout.Color), Color.Error}
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        DialogService.Show<Dialogs.Logout>(L["Logout"], parameters, options);
    }

    private void Profile()
    {
        Navigation.NavigateTo("/account");
    }
}