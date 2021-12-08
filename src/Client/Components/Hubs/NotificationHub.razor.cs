using System.Net;
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

    private HubConnection hubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        hubConnection = await TryConnectAsync().ConfigureAwait(true);
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        string apiBaseUri = _configurations.GetValue<string>("FullStackHero.API");
        hubConnection = hubConnection.TryInitialize(_localStorage, apiBaseUri);
        hubConnection.Closed += _hub_Closed;
        try
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
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

        return hubConnection;
    }

    private async Task _hub_Closed(Exception arg)
    {
        _snackBar.Add("SingalR Connection Closed.", MudBlazor.Severity.Error, a =>
        {
            a.RequireInteraction = true;
            a.ShowCloseIcon = true;
        });

        await Task.CompletedTask;
    }
}