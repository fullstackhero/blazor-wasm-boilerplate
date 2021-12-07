using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs;

public partial class NotificationHub
{
    [CascadingParameter]
    public Error Error { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = new RenderFragment(x => { });

    public HubConnection _hub { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _hub = await TryConnectAsync().ConfigureAwait(true);
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        string apiBaseUri = _configurations.GetValue<string>("FullStackHero.API");
        _hub = _hub.TryInitialize(_localStorage, apiBaseUri);
        _hub.Closed += _hub_Closed;
        try
        {
            if (_hub.State == HubConnectionState.Disconnected)
            {
                await _hub.StartAsync();
            }
        }
        catch (HttpRequestException requestException)
        {
            if (requestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                _snackBar.Add("SingalR Client Unauthorized.", MudBlazor.Severity.Error);
                await _authService.Logout();
            }
        }

        return _hub;
    }

    private async Task _hub_Closed(Exception arg)
    {
        _snackBar.Add("SingalR Disconnected.", MudBlazor.Severity.Error);
        await _authService.Logout();
    }
}